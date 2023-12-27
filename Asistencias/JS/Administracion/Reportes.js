$(document).ready(function () {
    ObtenerRecursos();
    LlenarMeses();
    LlenarAreas();
});

function ObtenerRecursos() {
    AjaxPost(
        '/api/v1/admin/recursos',
        null,
        function (respuesta) {
            console.log(respuesta);
        }
    );
}

function LlenarAreas() {
    $("#slct-area").append($('<option></option>').val(0).text('Seleccione ...'));
    AjaxPost(
        '/api/v1/admin/areas',
        null,
        function (respuesta) {
            console.log('areas');
            console.log(respuesta);
            if (respuesta.Estatus == 1) {
                $.each(respuesta.Data, function (i, v) {
                    $("#slct-area").append($('<option></option>').val(v.id).text(v.area));
                });
            }
        }
    );
}

//$(document).on('click', '#btn-filtro', ObtenerInfo());
$(document).on('change', '#slct-mes', function () { ObtenerInfo() });

function ObtenerInfo() {
    var mes = $("#slct-mes").val();
    var semana = $("#semana").val();
    var area = $("#slct-area").val();
    var no_empleado = $("#no_empleado").val();

    semana = semana.length == 0 ? '0' : semana;
    //no_empleado = no_empleado.length == 0 ? '0' : no_empleado;

    AjaxPost(
        '/api/v1/admin/info_mensual',
        { mes, semana, area, no_empleado },
        function (respuesta) {
            console.log('info');
            console.log(respuesta);
            if (respuesta.Estatus == 1) {
                $.each(respuesta.Data, function (i, v) {
                    $("#slct-area").append($('<option></option>').val(v.id).text(v.area));
                });
            }
        }
    );

}

function LlenarMeses() {
    $("#slct-mes").append($('<option></option>').val(0).text('Seleccione ...'));
    $.each(meses, function (i, v) {

        $("#slct-mes").append($('<option></option>').val(v.id).text(v.mes));
    });
}

var meses = [
    { id: 1, mes: 'Enero' },
    { id: 2, mes: 'Febrero' },
    { id: 3, mes: 'Marzo' },
    { id: 4, mes: 'Abril' },
    { id: 5, mes: 'Mayo' },
    { id: 6, mes: 'Junio' },
    { id: 7, mes: 'Julio' },
    { id: 8, mes: 'Agosto' },
    { id: 9, mes: 'Septiembre' },
    { id: 10, mes: 'Octubre' },
    { id: 11, mes: 'Noviembre' },
    { id: 12, mes: 'Diciembre' },

];