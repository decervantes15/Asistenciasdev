$(document).on('click', '#btn-buscar', function () {
    var username = $('#username').val();
    var inicio = $('#inicio').val();
    var fin = $('#fin').val();

    var aviso = $("#aviso");
    aviso.text('');

    if (inicio.length == 0) {
        aviso.text('Debe de ingresar la fecha de inicio');
        $("#inicio").focus();
    }
    else if (fin.length == 0) {
        aviso.text('Debe de ingresar la fecha de fin');
        $("#fin").focus();
    }
    else {
        var fecha_inicio = new Date(inicio);
        var fecha_fin = new Date(fin);
        if (fecha_inicio <= fecha_fin) {
            AjaxPost('/api/v1/asistencias',
                {
                    username: username,
                    fecha_inicio: inicio,
                    fecha_fin: fin,
                },
                function (respuesta) {
                    var html = '';
                    if (respuesta.Estatus == 1) {
                        var data = respuesta.Data;
                        var par_non = 0;

                        $.each(data, function (i, v) {
                            par_non++;
                            if (par_non % 2) {
                                html += '<tr class="bg-light">';
                            }
                            else {
                                html += '<tr class="bg-gray">';
                            }
                            html += "<td class='fn-8'>" + v.no_empleado + "</td>";
                            html += "<td class='fn-8'>" + v.email + "</td>";
                            html += "<td class='fn-8'>" + v.recurso + "</td>";
                            html += "<td class='fn-8'>" + v.ip + "</td>";
                            html += "<td class='fn-8'>" + v.area + "</td>";
                            html += "<td class='fn-8'>" + v.equipo + "</td>";
                            html += "<td class='fn-8'>" + v.rol + "</td>";
                            html += "<td class='fn-8'>" + v.puesto + "</td>";
                            html += "<td class='fn-8'>" + v.comentarios + "</td>";
                            html += "<td class='fn-8'>" + v.entrada + "</td>";
                            html += "<td class='fn-8'>" + v.comida_inicio + "</td>";
                            html += "<td class='fn-8'>" + v.comida_fin + "</td>";
                            html += "<td class='fn-8'>" + v.salida + "</td>";
                            html += '</tr>';
                        });

                    }
                    else {
                        aviso.text(respuesta.Mensaje);
                    }
                    $('#data').html(html);
                }
            );
        }
        else {
            aviso.text('Las fechas son incorrectas');
        }
    }
});

$(document).on('click', '#btn-exportar', function () {
    var username = $('#username').val();
    var inicio = $('#inicio').val();
    var fin = $('#fin').val();

    var aviso = $("#aviso");
    

    if (inicio.length == 0) {
        aviso.text('Debe de ingresar la fecha de inicio');
        $("#inicio").focus();
    }
    else if (fin.length == 0) {
        aviso.text('Debe de ingresar la fecha de fin');
        $("#fin").focus();
    }
    else {
        var fecha_inicio = new Date(inicio);
        var fecha_fin = new Date(fin);
        if (fecha_inicio <= fecha_fin) {
            window.open('/Administracion/ExportarConsulta?username=' + username + "&inicio=" + inicio + "&fin=" + fin, '_blank');
        }
        else {
            aviso.text('Las fechas son incorrectas');
        }
    }
});

$(document).on('click', '.salir', function () {
    window.location.href = "/Home/Salir";
});
