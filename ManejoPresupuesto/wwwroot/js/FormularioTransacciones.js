
function InicializarFormularioTransacciones(urlObtenerCategorias) {
    $("#TipoOperacionId").change(async function () {
        const valorSeleccionado = $(this).val();

        const respuesta = await fetch(urlObtenerCategorias, {
            method: 'POST',
            body: valorSeleccionado,
            headers: {
                'Content-Type': 'application/json'
            }
        });

        //El json recibe las categorias 
        const json = await respuesta.json();

        //en opciones guardamos los categorias en elementos de un dropdownlist con su Id y texto
        const opciones = json.map(categoria => `<option value=${categoria.value}>${categoria.text}</option>`);

        // y por ultimo le damos esos elementos al dropdownlist para que cargue las nuevas opciones
        $('#CategoriaId').html(opciones);
    })
}