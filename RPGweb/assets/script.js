let forca = document.querySelector('#forca');

forca.addEventListener('click', function() {
    let numeroAleatorio = Math.floor(Math.random() * 20);
    alert(numeroAleatorio);
});