(function () {
    $(function () {
        var searchDebounceProjectUrl = "/OutRequests/SearchDebounceProject";

        var showProjectListUrl = "/OutRequests/ShowProjectList";


        $('#btnShowProjectList').click(function (e) {
            var test = 0;
            $.ajax({
                url: showProjectListUrl,
                type: "POST",
                cache: false,
                data: { q: $("#searchBoxArticulo").val() },
                success: function (data) {
                    $('#anyModalForm').html(data);
                    //$(this).addClass("done");
                    $('#anyModalForm').modal('show');
                }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
            });
            return false;
        });

        this.searchDebounceProject = function (e) {
            $.ajax({
                url: searchDebounceProjectUrl,
                type: "POST",
                cache: false,
                data: { query: $("#searchBoxProject").val() },
                success: function (data) {
                    $("#anyListUpdate").html(data);
                }, error: function (err) { writeError("IndexAlerts", "¡Error al consultar los Proyectos!", "error"); }
            });
            return false;
        };
    });
})();