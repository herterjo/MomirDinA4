using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using MomirDinA4.Pdf;
using MomirDinA4.ScryfallApiObjects;
using Newtonsoft.Json;

namespace MomirDinA4;

public static class WebServer
{
    private readonly static Dictionary<string, string> MimeTypes = new()
    {
        { ".html", "text/html" },
        { ".css", "text/css" },
        { ".js", "text/javascript" }
    };

    public static void Start()
    {
        using var listener = new HttpListener();
        var webserverListeningAddress = "http://" + Config.Instance.Host + ":" + Config.Instance.Port + "/";
        listener.Prefixes.Add(webserverListeningAddress);
        listener.Start();

        Console.WriteLine("Web Server Running on address " + webserverListeningAddress + "... Press ^C to Stop...");

        while (true)
        {
            var ctx = listener.GetContext();

            switch (ctx.Request.HttpMethod?.ToUpper())
            {
                case "GET":
                    AnswerWithFile(ctx);
                    break;
                case "POST":
                    AnswerWithMethod(ctx);
                    break;
                default:
                {
                    using var response = ctx.Response;
                    ErrorMessage(response, "Only GET and POST supported", HttpStatusCode.NotAcceptable);
                    break;
                }
            }
        }
    }

    private static void AnswerWithMethod(HttpListenerContext ctx)
    {
        switch (ctx.Request.Url?.LocalPath.Trim('/'))
        {
            case "downloadBasicPrintPdf":
                GeneratePdf<BasicPrintPdfRequest>(ctx, Generator.GetBasicPrintPdf);
                break;
            case "downloadCmcPdf":
                GeneratePdf<CmcPdfRequest>(ctx, Generator.GetCmcPdf);
                break;
            case "getCmcCounts":
                GetCmcCounts(ctx);
                break;
            default:
            {
                using var response = ctx.Response;
                ErrorMessage(response, "Method not supported", HttpStatusCode.NotAcceptable);
                break;
            }
        }
    }

    private static void GeneratePdf<TRequestBody>(HttpListenerContext ctx, Func<TRequestBody, Response> generationFunc)
    {
        using var response = ctx.Response;
        using var sr = new StreamReader(ctx.Request.InputStream);
        var requestBody = sr.ReadToEnd();
        var deserialized = JsonConvert.DeserializeObject<TRequestBody>(requestBody);
        if (deserialized == null)
        {
            ErrorMessage(response, "No request body received", HttpStatusCode.NotAcceptable);
            return;
        }
        var responseBody = generationFunc(deserialized);
        SendJson(response, responseBody);
    }

    private static void GetCmcCounts(HttpListenerContext ctx)
    {
        using var response = ctx.Response;

        var cmcCounts = ScryfallApi.CmcCards?.Keys.OrderBy(i => i).ToList();
        SendJson(response, cmcCounts);
    }

    private static void SendJson(HttpListenerResponse response, object? toSend)
    {
        var cmcCountsJson = JsonConvert.SerializeObject(toSend);
        var ebuf = Encoding.UTF8.GetBytes(cmcCountsJson);

        response.StatusCode = (int)HttpStatusCode.OK;

        response.ContentLength64 = ebuf.Length;

        response.Headers.Set("Content-Type", "application/json");

        using var ros = response.OutputStream;
        ros.Write(ebuf, 0, ebuf.Length);
    }

    private static void AnswerWithFile(HttpListenerContext ctx)
    {
        using var response = ctx.Response;
        var path = ctx.Request.Url?.LocalPath.Trim('/');

        if (String.IsNullOrWhiteSpace(path))
        {
            path = "index.html";
        }

        if (path.Contains(".."))
        {
            ErrorMessage(response, "Path invalid", HttpStatusCode.NotAcceptable);
            return;
        }

        path = "Frontend/" + path;

        if (!File.Exists(path))
        {
            ErrorMessage(response, "Not found", HttpStatusCode.NotFound);
            return;
        }

        using var fileStream = File.OpenRead(path);
        using var ros = response.OutputStream;

        response.StatusCode = (int)HttpStatusCode.OK;
        response.ContentLength64 = fileStream.Length;

        var extension = Path.GetExtension(path);
        if (String.IsNullOrWhiteSpace(extension) || !MimeTypes.TryGetValue(extension, out var mimeType))
        {
            mimeType = "text/plain";
        }
        response.Headers.Set("Content-Type", mimeType);

        fileStream.CopyTo(ros);
    }

    private static void ErrorMessage(HttpListenerResponse response, string message, HttpStatusCode statusCode)
    {
        response.Headers.Set("Content-Type", "text/plain");

        using var ros = response.OutputStream;

        response.StatusCode = (int)statusCode;

        var ebuf = Encoding.UTF8.GetBytes(message);
        response.ContentLength64 = ebuf.Length;

        ros.Write(ebuf, 0, ebuf.Length);
    }
}
