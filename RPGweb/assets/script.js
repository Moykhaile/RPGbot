let rolarDados = document.querySelectorAll('.rolar-dado');
let inf = document.querySelector('#inf');
let inv = document.querySelector('#inv');
let com = document.querySelector('#com');
let aparecerinfo = document.querySelector('#aparecerinfo');
let aparecerinv = document.querySelector('#aparecerinv');
let combate = document.querySelector('#combate');
let overlay = document.querySelector(".overlay");
let closeButtons = document.querySelectorAll(".close");

// Função fechar
function fecharModal() {
    aparecerinfo.style.display = 'none';
    aparecerinv.style.display = 'none';
    overlay.style.display = 'none';
    combate.style.display = 'none';
}

// Fechar
closeButtons.forEach(function (button) {
    button.addEventListener('click', fecharModal);
});

// Mostrar info
inf.addEventListener('click', function () {
    aparecerinfo.style.display = 'block';
    overlay.style.display = 'block';
});

// Mostrar inv
inv.addEventListener('click', function () {
    aparecerinv.style.display = 'block';
    overlay.style.display = 'block';
});

// Mostrar combate
com.addEventListener('click', function () {
    combate.style.display = 'block';
    overlay.style.display = 'block';
});

rolarDados.forEach(function (button) {
    button.addEventListener('click', function () {
        let tipo = this.getAttribute('data-tipo');
        let numeroAleatorio;

        switch (tipo) {
            case "d20":
                numeroAleatorio = Math.floor(Math.random() * 20 + 1);
                break;
            case "d20-1":
                numeroAleatorio = Math.floor(Math.random() * 19 + 1);
                break;
            case "d20+1":
                numeroAleatorio = Math.floor(Math.random() * 21 + 1);
                break;
            case "d20+2":
                numeroAleatorio = Math.floor(Math.random() * 22 + 1);
                break;
            case "d20+3":
                numeroAleatorio = Math.floor(Math.random() * 23 + 1);
                break;
            case "d12":
                numeroAleatorio = Math.floor(Math.random() * 12 + 1);
                break;
            case "d10":
                numeroAleatorio = Math.floor(Math.random() * 10 + 1);
                break;
            case "d8":
                numeroAleatorio = Math.floor(Math.random() * 8 + 1);
                break;
            case "d6":
                numeroAleatorio = Math.floor(Math.random() * 6 + 1);
                break;
            case "d4":
                numeroAleatorio = Math.floor(Math.random() * 4 + 1);
                break;
            default:
                console.error("Tipo de dado não reconhecido");
                return;
        }

        console.log(`Rolou ${tipo}: ${numeroAleatorio}`); // Diagnóstico
        alert(numeroAleatorio);
    });
});