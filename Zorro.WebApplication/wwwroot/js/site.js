// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
const validationClasses = ['is-invalid', 'is-valid']

function verify(inputId, route, buttonId) {
    try {
        let uri = route + '?' + new URLSearchParams({
            id: document.getElementById(inputId).value,
        })
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

async function verifyBillerCode(inputId, buttonId) {
    let status = 1;
    let billerInput = document.getElementById(inputId);
    let billerCode = billerInput.value;

    try {
        let billerName = await getBpayBillerName(billerCode);
        document.getElementById('billerName').value = billerName;
    } catch (e) {
        status = 0;
    }
    
    showVerificationStatus(status, inputId);
    //enableOrDisableButton(status, buttonId)
}

async function getBpayBillerName(billerCode) {
    let uri = "VerifyBillerCode" + '?' + new URLSearchParams({
        id: billerCode,
    });

    let resp = await fetch(uri);
    let data = await resp.json();
    return data['billerName'];
}

function showVerificationStatus(status, elementId) {
    const inputElement = document.getElementById(elementId);
    resetValidation(inputElement)
    inputElement.classList.add(validationClasses[status]);
}

function resetValidation(input) {
    if (input == null)
        return;
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