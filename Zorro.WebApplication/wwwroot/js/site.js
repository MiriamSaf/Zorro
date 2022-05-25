//JS for site
const validationClasses = ['is-invalid', 'is-valid']
const transferLimit = 10000;

async function verifyWallet(inputId, buttonId = null) {
    let walletId = document.getElementById(inputId).value;
    var result = await verifyWalletWithServer(walletId);
    var status = result ? 1 : 0;
    showVerificationStatus(status, inputId)
    if (buttonId == null)
        return
    enableOrDisableButton(status, buttonId);
}

async function verifyWalletWithServer(walletId) {
    // build uri with query string
    let route = "VerifyWalletId";
    let uri = route + '?' + new URLSearchParams({
        id: walletId,
    });

    // make request to server to verify wallet ID
    try {
        let result = await fetch(uri);
        if (result.ok)
            return true;
        else
            return false;
    } catch {
        console.error("An error occurred during wallet validation");
        return false;
    }
}

//verify biller code
async function verifyBillerCode(inputId, billerNameInputId, buttonId = null) {
    let status = 1;
    let billerInput = document.getElementById(inputId);
    let billerNameInput = document.getElementById(billerNameInputId)
    let billerCode = billerInput.value;

    let billerName = ""
    try {
        billerName = await getBpayBillerName(billerCode);
    } catch (e) {
        console.error("An error when verifying biller code");
        status = 0;
    }
    billerNameInput.value = billerName;

    showVerificationStatus(status, inputId);
    if (buttonId == null) {
        return;
    }
    enableOrDisableButton(status, buttonId);
}

//get biller name
async function getBpayBillerName(billerCode) {
    let uri = "VerifyBillerCode" + '?' + new URLSearchParams({
        id: billerCode,
    });

    let resp = await fetch(uri);
    let data = await resp.json();
    return data['billerName'];
}

//show status
function showVerificationStatus(status, elementId) {
    const inputElement = document.getElementById(elementId);
    resetValidation(inputElement)
    inputElement.classList.add(validationClasses[status]);
}

//reset validation 
function resetValidation(input) {
    if (input == null)
        return;
    input.classList.remove(validationClasses[0], validationClasses[1])
}

//enable or disable buttons by passing button id
function enableOrDisableButton(status, buttonId) {
    let transferButton = document.getElementById(buttonId);
    if (status == 1) {
        transferButton.classList.remove("disabled");
    }
    else {
        transferButton.classList.add("disabled");
    }
}

//formar amount of passed in input
function formatAmount(input) {
    if (!input.hasAttribute('data-previous-amount')) {
        input.dataset.previousAmount = 0;
    }
    let newValue = parseFloat(input.value).toFixed(2);
    if (newValue > 10000) {
        input.value = input.dataset.previousAmount;
    }
    input.dataset.previousAmount = input.value;
    let decimalIndex = input.value.indexOf('.');
    if (decimalIndex == -1) {
        return;
    }
    if (input.value.length > decimalIndex + 2) {
        input.value = newValue;
    }
}

//ignores hyphen and e keys
function ingoreHyphenAndEKeys(event) {
    return event.keyCode !== 69 && event.keyCode !== 189
}