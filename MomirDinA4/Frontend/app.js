"use strict";

const alreadyPrintedCards = [];

$(() => {
	sendRequest("getCmcCounts", null, setCmcCounts);
});

function collectPdfSettings() {
	const settings = {
		DuplexPrintingEnabled: document.getElementById("DuplexPrintingEnabled").checked,
		IncludeQrCode: document.getElementById("IncludeQrCode").checked,
		CropArt: document.getElementById("CropArt").checked,
        NoArt: document.getElementById("NoArt").checked
	};
	return settings;
}

const b64toBlob = (b64Data, contentType = '', sliceSize = 512) => {
    const byteCharacters = atob(b64Data);
    const byteArrays = [];

    for (let offset = 0; offset < byteCharacters.length; offset += sliceSize) {
        const slice = byteCharacters.slice(offset, offset + sliceSize);

        const byteNumbers = new Array(slice.length);
        for (let i = 0; i < slice.length; i++) {
            byteNumbers[i] = slice.charCodeAt(i);
        }

        const byteArray = new Uint8Array(byteNumbers);
        byteArrays.push(byteArray);
    }

    const blob = new Blob(byteArrays, { type: contentType });
    return blob;
}

function download(toDownload, filename) {
    const element = document.createElement('a');

    var blob = b64toBlob(toDownload, 'application/pdf');

    const URL = window.URL || window.webkitURL;
    const downloadUrl = URL.createObjectURL(blob);

    // set object URL as the anchor's href
    element.href = downloadUrl;
    element.setAttribute('download', filename);

    element.style.display = 'none';
    element.setAttribute("target", "_blank")
    document.body.appendChild(element);

    element.click();

    document.body.removeChild(element);
}

function downloadCards(response, filename) {
    response = JSON.parse(response);
    response.IncludedCards.forEach(c => alreadyPrintedCards.push(c));
    download(response.PdfContent, filename);
}

function downloadBasicPrintPdf() {
    const downloadSettings = {
        pdfSettings: collectPdfSettings(),
        momirEmblemCount: getIntFromInput(document.getElementById("MomirEmblemCount")),
        initialCmcs: [],
        alreadyPrintedCards: document.getElementById("PreventDuplicates").checked ? alreadyPrintedCards : []
    };
    Array.from(document.getElementsByClassName(initialCmcButtonClass)).forEach(checkbox => {
        if (!checkbox.checked) {
            return;
        }
        const intCmc = parseInt(checkbox.value);
        if (intCmc || intCmc === 0) {
            downloadSettings.initialCmcs.push(checkbox.value);
        }
    });

    sendRequest("downloadBasicPrintPdf", downloadSettings, response => {
        downloadCards(response, "initial.pdf");
    });
}

function downloadCmcPdf(cmc) {
    const downloadSettings = {
        pdfSettings: collectPdfSettings(),
        cmc: cmc,
        alreadyPrintedCards: document.getElementById("PreventDuplicates").checked ? alreadyPrintedCards : []
    };

    sendRequest("downloadCmcPdf", downloadSettings, response => {
        downloadCards(response, cmc + ".pdf");
    });
}

function getIntFromInput(input) {
    const stringValue = input.value;
    return parseInt(stringValue);
}

function parseInt(stringValue) {
    if (stringValue === null || stringValue === undefined || stringValue === "") {
        return null;
    }
    return Number.parseInt(stringValue);
}

function sendRequest(suburl, body, onDone) {
	const xhr = new XMLHttpRequest();
    xhr.open("POST", suburl, true);

    xhr.onreadystatechange = function () {
        if (this.readyState == 4 && this.status == 200) {
            onDone(this.responseText);
        }
    }
    // Sending our request 
    xhr.send(JSON.stringify(body));
}

const initialCmcButtonClass = "initial-cmc";
function setCmcCounts(response) {
    const cmcCounts = JSON.parse(response);
    const initialContainer = document.getElementById("InitialCmcs");
    const additionalPagesContainer = document.getElementById("AdditionalPages");

    for (let i = 0; i < cmcCounts.length; i++) {
        const labelInputContainer = document.createElement("div");
        labelInputContainer.className = "initial-cmc-container";

        const cmc = cmcCounts[i];

        const cmcId = "InitialCmc" + cmc;
        const cmcLabel = document.createElement("label");
        cmcLabel.for = cmcId;
        cmcLabel.innerText = cmc;
        labelInputContainer.appendChild(cmcLabel);

        const cmcCheckbox = document.createElement("input");
        cmcCheckbox.id = cmcId;
        cmcCheckbox.type = "checkbox";
        cmcCheckbox.value = cmc;
        cmcCheckbox.checked = cmc > 0 && cmc < 14;
        cmcCheckbox.className = initialCmcButtonClass;
        labelInputContainer.appendChild(cmcCheckbox);

        initialContainer.appendChild(labelInputContainer);

        const cmcButton = document.createElement("button");
        cmcButton.className = "cmc-button";
        cmcButton.textContent = cmc;
        const cmcCopy = cmc;
        cmcButton.onclick = () => downloadCmcPdf(cmcCopy);

        additionalPagesContainer.appendChild(cmcButton);
	}
}