document.addEventListener('DOMcontentLoaded', setTimeout(function () { AuthFormPopup(true); }, 5000));

function AuthFormPopup(isRegister) {
    if (isRegister === true) {
        document.getElementById('popupdiv').style.display = 'block'
        document.getElementById('clientregdiv').style.display = 'block';
    }
    else {
        document.getElementById('popupdiv').style.display = 'block'
        document.getElementById('userlogindiv').style.display = 'block';
    }
}

function AuthFormClose() {
    document.getElementById('popupdiv').style.display = 'none';
    document.getElementById('clientregdiv').style.display = 'none';
    document.getElementById('employerregdiv').style.display = 'none';
    document.getElementById('userlogindiv').style.display = 'none';
}

function AuthFormSwitch(isClient) {
    if (isClient == true) {
        document.getElementById('employerregdiv').style.display = 'none';
        document.getElementById('clientregdiv').style.display = 'block';
    }
    else {
        document.getElementById('clientregdiv').style.display = 'none';
        document.getElementById('employerregdiv').style.display = 'block';
    }
}