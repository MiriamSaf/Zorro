// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const validationClasses = ['is-invalid', 'is-valid']

function verify(inputId, route, buttonId) {
    try {
        let uri = route + '?' + new URLSearchParams({
            id: document.getElementById(inputId).value,
        })
        console.log(uri)
        fetch(uri).then(function (response) {
            let status = 0
            if (response.ok) {
                status = 1;
            }
            showVerificationStatus(status, inputId);
            enableOrDisableButton(status, buttonId);
        })

    } catch (e) {
        showVerificationStatus(0, inputId);
        console.error('Unable to verify', error)
    }
}

function showVerificationStatus(status, elementId) {
    const inputElement = document.getElementById(elementId);
    resetValidation(inputElement)
    inputElement.classList.add(validationClasses[status]);
}

function resetValidation(input) {
    input.classList.remove(validationClasses[0], validationClasses[1])
}

function enableOrDisableButton(status, buttonId) {
    let transferButton = document.getElementById(buttonId);
    if (status == 1) {
        transferButton.classList.remove("disabled");
    }
    else {
        transferButton.classList.add("disabled");
    }
}

const verifyWalletButton = document.getElementById("recipientWalletButton");
verifyWalletButton.addEventListener("click", function () { verify('recipientWalletInput', 'VerifyWalletId', 'transferButton') }, false);

const walletInput = document.getElementById("recipientWalletInput");
walletInput.addEventListener("blur", function () { verify('recipientWalletInput', 'VerifyWalletId', 'transferButton') }, false);