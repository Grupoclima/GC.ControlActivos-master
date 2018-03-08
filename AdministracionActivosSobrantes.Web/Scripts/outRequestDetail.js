function ViewSalidaInventarioProcess() {
    var createUrl = "/OutRequests/Create";
    var searchUrl = "/OutRequests/AjaxPage";
    var searchDebounceUrl = "/OutRequests/SearchDebounce";
    var outRequestModel;

    var searchArticuloSolicitudUrl = "/OutRequests/SearchArticulos";
    var showArticuloListUrl = "/OutRequests/ShowArticuloList";
    var renderDetalleArticuloListUrl = "/OutRequests/RenderListDetallesOrdenDetail";;
    var addAndRenderDetalleArticuloListUrl = "/OutRequests/AddAndRenderListDetallesOrden";

    var outRequest = function (outRequestId, status,arrayDetalles) {
        this.Id = outRequestId;
        this.DetailsRequest = arrayDetalles;
        this.Status = status;
    };

    var detailOutRequest = function (detailId, assetId, nameAsset, stockAsset, price, availability, previosQty, saved, update, deletep, errorCode, errorDesc, index, status) {
        this.Index = index;
        this.DetailId = detailId;
        this.AssetId = assetId;
        this.NameAsset = nameAsset;
        this.StockAsset = stockAsset;
        this.PreviousQty = previosQty;
        this.Price = price;
        this.Status = status;
        this.Saved = saved; //Si ya esta guardado a a solicitud
        this.Update = update; //Si hay que actualizar algun dato
        this.Delete = deletep; // Si hay que actualizar algun dato
        this.AssetAvailability = availability;
        this.ErrorDescription = errorDesc;
        this.ErrorCode = errorCode;
    };

    // Main Search, typing timer debouce
    this.searchDebounce = function (e) {
        $.ajax({
            url: searchDebounceUrl,
            type: "POST",
            cache: false,
            data: { query: $("#searchBox").val() },
            success: function (data) {
                $("#anyListEntity").html(data);
            }, error: function (err) { writeError("IndexAlerts", "¡Error al consultar las Solicitudes!", "error"); }
        });
        return false;
    };

    this.addDetailOutRequest = function (detailId, assetId, nameAsset, stockAsset, price, availability, previousQty, saved, update, deletep, errorCode, errorDesc, index, status) {
        try {
            nameAsset = $("<div>").html(nameAsset).text();
            var requestDetailItem = new detailOutRequest(detailId, assetId, nameAsset, stockAsset, price, availability, previousQty, saved, update, deletep, errorCode, errorDesc, index, status);
            outRequestModel.DetailsRequest.push(requestDetailItem);
            return requestDetailItem;
        } catch (exc) { return false; }
    };

    this.removeDetailOutRequest = function (index) {
        try {
            $.each(outRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    value.Delete = 1;
                    value.Update = 1;
                }
            });
            renderDetailAssetList();
            return true;
        }
        catch (exc) { return false; }
    };

    function getIndexDetailOutRequest(index) {
        var detalle;
        try {
            $.each(outRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    detalle = value;
                }
            });
            return detalle;
        }
        catch (exc) { return -1; }
    };

    this.editDetailOutRequest = function (index, assetId, assetName, stock, price, availability) {
        try {
            $.each(outRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    //value.Update = 1;
                    value.AssetId = assetId;
                    value.NameAsset = assetName;
                    value.StockAsset = stock;
                    value.Price = price;
                    value.AssetAvailability = availability;
                    value.Update = 2; // si hay que actualizar algun dato
                }
            });
            renderDetailAssetList();
            return true;
        }
        catch (exc) { return false; }
    };

    this.clearDetailOutRequestList = function () {
        try {
            outRequestModel.DetailsRequest = [];
        } catch (exc) { return false; }
    };


    this.setOutRequest = function (outRequestId, status) {
        try {
            outRequestModel.Id = outRequestId;
            outRequestModel.Status = status;
            return true;
        } catch (exc) { return false; }
    };

    this.getOutRequest = function () {
        try {
            return outRequestModel;
        } catch (exc) {
            return false;
        }
    };

    this.initializeOutRequest = function () {
        try {
            outRequestModel = new outRequest('', []);
            return outRequestModel;
        } catch (exc) {
            return false;
        }
    };

    //---- Errors Alerts
    function clearErrors() { $('#msgErrorAnyModal').html(''); $('#IndexAlerts').html(''); $('#detalleInventarioAlerts').html(''); }

    function writeError(control, msg, type) {
        if (type === "success") {
            abp.notify.success(msg, "");
        } else if (type === "error") {
            abp.notify.error(msg, "");
            var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
            $("#" + control).html(alert);
        } else { abp.notify.warn(msg, ""); }
    }

    //------------Articulos y Detalles
    function getRequest(url) {
        $.ajax({
            url: url,
            context: document.body,
            success: function (data) {
                $('.modal-body p.body').html(data);
                $(this).addClass("done");
                $('#anyModalForm').modal('show');
            }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
        });
    }

    function isEmpty(str) { return (!str || 0 === str.length); }

    $('.closeModal').click(function (e) { $('#anyModalForm').modal('hide'); });

    function renderDetailAssetList() {
        clearErrors();
        $.ajax({
            url: renderDetalleArticuloListUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest) },
            success: function (data) {
                if (data.length == null) { writeError('detalleInventarioAlerts', data.result.message, 'error'); }
                else {
                    $("#anyListDetailsEntity").html(data);
                    clearDetailsFields();
                    disableDetailsFields();
                }
            }, error: function () { writeError('detalleInventarioAlerts', data.result.message, 'error'); }
        });
    }


    function clearDetailsFields() {
        $(".clearDetalle").val('');
        return true;
    }

    function enableDetailsFields() {
        $(".detalleAddPanel").prop('disabled', false);
        return true;
    }
    function disableDetailsFields() {
        $(".detalleAddPanel").prop('disabled', true);
        return true;
    }


    this.updateRequestsList = function () {
        updateRequestsListLocal();
    };

    function updateRequestsListLocal() {
        $('body').addClass("loading");
        $("#anyPanelBody").html('');
        $.ajax({
            url: searchUrl,
            type: "POST",
            cache: false,
            data: { query: $('#SrchQuery_hidden').val(), page: $('#SrchPage_hidden').val() },
            success: function (data) {
                $("#anyListEntity").html(data);
                $('body').removeClass("loading");
            },
            error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al buscar la Solicitud', 'error'); $('body').removeClass("loading"); }
        });
        return false;
    }


}

ViewOutRequestVariable = new ViewSalidaInventarioProcess();