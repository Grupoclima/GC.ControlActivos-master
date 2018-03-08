(function () {
    $(function () {
       
        var createUrl = "/Project/Create";
        var editUrl = "/Project/Edit";
        var detailUrl = "/Project/Details";
        var deleteUrl = "/Project/Delete";
        var searchUrl = "/Project/AjaxPage";

        var okFlag = 1;
        var noneFlag = 0;
        var errorFlag = -1;
        
        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }
        function clearErrors() { $("#msgErrorAnyModal").html(""); $("#IndexAlerts").html(""); }
        
        function getRequest(url) {
            clearErrors();
            abp.ui.setBusy();
            $.ajax({
                url: url,
                context: document.body,
                success: function (data) {
                    $(".modal-body p.body").html(data);
                    //$(this).addClass("done");
                    $("#anyModalForm").modal("show");
                    abp.ui.clearBusy();
                }, error: function(err) {
                    writeError("msgErrorAnyModal", "¡Error al consultar los datos de los Proyectos!","error");
                    abp.ui.clearBusy();
                }
            });
        }

        function isEmpty(str) { return (!str || 0 === str.length); }
        $(".closeModal").click(function (e) { $("#anyModalForm").modal("hide"); });


        //--- Butons Events Functions
        

        this.editFunc=function (btn) {
            clearErrors();
            if (!isEmpty(editUrl)) {
                var id = btn.attr("data-idEntity");
                var url = editUrl + "/" + id;
                getRequest(url);
            }
            return false;
        }

        this.detailFunc = function (btn) {
            clearErrors();
            if (!isEmpty(detailUrl)) {
                var id = btn.attr("data-idEntity");
                var url = detailUrl + "/" + id;
                getRequest(url);
            }
            return false;
        }

        this.deleteFunc = function (btn) {
            clearErrors();
            $("#DeleteConfirmation_modal").modal("show");
            $("#ItemToDelete_hidden").val(btn.attr("data-idEntity"));
            return false;
        }
      
        //Get and refresh list of entities from server
        function refreshList() {
            //abp.ui.setBusy();
            $.ajax({
                url: searchUrl,
                type: "GET",
                cache: false,
                data: { query: $("#SrchQuery_hidden").val(), page: $("#SrchPage_hidden").val() },
                success: function (data) {
                    $("#anyListEntity").html(data);

                    //$("#PartialView").find("#button").click(function () {
                    //    alert('button clicked');
                    //});
                    setAllEvents();

                    //abp.ui.clearBusy();
                }, error: function (xhr, ajaxOptions, thrownError) {
                    writeError("IndexAlerts", "¡Error al consultar los Proyectos!", "error");
                    // abp.ui.clearBusy();
                }
            });
            return false;
        };

        //send to server and render views, when submit information from edit, create, details
        $("#anyModalForm").submit(function (event) {
            clearErrors();
            abp.ui.setBusy();
            var action = $("#form").attr("action");
            $.post(action, $("#form").serialize(), function (data, status) {
                $("#anyModalBody").html(data);
                abp.ui.clearBusy();
                refreshList();//refresh list
            }).error(function (error, status) {
                writeError("msgErrorAnyModal", "Error procesando datos. ¡Por favor revise los datos nuevamente!", "error");
                $(".modal-body p.body").html(error.responseText);
                abp.ui.clearBusy();
            });
            return false;
        });

        //send to server and render views, when submit information from search
        $("#searchForm").submit(function (event) {
            clearErrors();
            abp.ui.setBusy();
            var action = $("#searchForm").attr("action");
            $.post(action, $("#searchForm").serialize(), function (data, status) {
                $("#anyListEntity").html(data);
                abp.ui.clearBusy();
            }).error(function (error, status) {
                writeError("IndexAlerts", "Error procesando datos. ¡Por favor revise los datos nuevamente!", "error");
                abp.ui.clearBusy();
            });
            return false;
        });

        //Close
        $("a.btnCloseFormSimple").click(function (e) {
            $("#anyModalForm").modal("hide");
            return false;
        });

        //Cellar Create
        $("a.btnCreateForm").click(function (e) {
            clearErrors();
            if (!isEmpty(createUrl)) {
                getRequest(createUrl);
            }
            return false;
        });


        $("#btnModalOkDeleteConfirmation").click(function (e) {
            $("#DeleteConfirmation_modal").modal("hide");
            abp.ui.setBusy();
            if (!isEmpty(deleteUrl)) {
                var id = $("#ItemToDelete_hidden").val();
                $.ajax({
                    url: deleteUrl + "/",
                    type: "DELETE",
                    cache: false,
                    data: { id: id },
                    success: function (data) {
                        if (data=== okFlag) {
                            writeError("IndexAlerts", "Proyecto eliminado correctamente", "success");
                            //$("#tableRowId" + id).remove();
                            refreshList();
                            abp.ui.clearBusy();
                        } else if (data < noneFlag) {
                            writeError("IndexAlerts", "Error al eliminar el proyecto", "error");
                            abp.ui.clearBusy();
                        }
                    }, error: function(xhr, ajaxOptions, thrownError) {
                        writeError("IndexAlerts", "¡Error al eliminar el Proyecto!", "error");
                        abp.ui.clearBusy();
                    }
                });
            } return false;
        });
        $("#ReturnUrlHash").val(location.hash);
        $("#LoginForm input:first-child").focus();
    });
})();

function ViewProjectProcess() {
    var searchDebounceUrl = "/Project/SearchDebounce";
    function writeError(control, msg, type) {
        if (type === "success") {
            abp.notify.success(msg, "");
        } else if (type === "error") {
            abp.notify.error(msg, "");
            var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
            $("#" + control).html(alert);
        } else { abp.notify.warn(msg, ""); }
    }
    // Main Search, typing timer debouce
    this.searchDebounce = function () {
        $.ajax({
            url: searchDebounceUrl,
            type: "POST",
            cache: false,
            data: { query: $("#MainSearchTextInput").val() },
            success: function (data) {
                $("#anyListEntity").html(data);
            }, error: function (err) { writeError("IndexAlerts", "¡Error al consultar los proyectos!", "error"); }
        });
        return false;
    };
}
ViewProjectVariable = new ViewProjectProcess();