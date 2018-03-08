function ViewSalidaInventarioProcess() {
    var createUrl = "/OutRequests/Create";
    var searchUrl = "/OutRequests/AjaxPage";
    var outRequestModel;
    var closeRequestPartialUrl = "/OutRequests/ApplyMovementCloseRequest";
    var renderDetalleArticuloListUrl = "/OutRequests/RenderListDetailsCloseRequest";;

    var outRequest = function (outRequestId, cellarId, arrayDetalles) {
        this.Id = outRequestId;
        this.CellarId = cellarId;
        this.DetailsRequest = arrayDetalles;
    };

    var detailOutRequest = function (detailId, assetId, nameAsset, stockAsset, price, returnAssetQty, leftOver, assetReturnPartialQty, outRequestId, index) {
        this.Index = index;
        this.DetailId = detailId;
        this.AssetId = assetId;
        this.NameAsset = nameAsset;
        this.StockAsset = stockAsset;
        this.Price = price;
        this.OutRequestId = outRequestId;
        this.ReturnAssetQty = returnAssetQty;
        this.AssetReturnPartialQty = assetReturnPartialQty;
        this.LeftOver = leftOver;
    };

    this.addDetailOutRequest = function (detailId, assetId, nameAsset, stockAsset, price, returnAssetQty, leftOver, assetReturnPartialQty, outRequestId,index) {
        try {
            nameAsset = $("<div>").html(nameAsset).text();
            var requestDetailItem = new detailOutRequest(detailId, assetId, nameAsset, stockAsset, price, returnAssetQty, leftOver, assetReturnPartialQty, outRequestId, 0);
            outRequestModel.DetailsRequest.push(requestDetailItem);
            return requestDetailItem;
        } catch (exc) {
            return false;
        }
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
        } catch (exc) {
            return -1;
        }
    };

    this.clearDetailOutRequestList = function () {
        try {
            outRequestModel.DetailsRequest = [];
        } catch (exc) {
            return false;
        }
    };


    this.setOutRequest = function (outRequestId, cellarId) {
        try {
            outRequestModel.Id = outRequestId;
            outRequestModel.CellarId = cellarId;
            return true;
        } catch (exc) {
            return false;
        }
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
            outRequestModel = new outRequest('', '', []);
            return outRequestModel;
        } catch (exc) {
            return false;
        }
    };


    //---- Errors Alerts
    function clearErrors() {
        $('#msgErrorAnyModal').html('');
        $('#IndexAlerts').html('');
        $('#detalleInventarioAlerts').html('');
        $('#closeRequestAlerts').html('');
    }

    function writeError(control, msg, type) {
        if (type === "success") {
            abp.notify.success(msg, "");
        } else if (type === "error") {
            abp.notify.error(msg, "");
            var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
            $("#" + control).html(alert);
        } else {
            abp.notify.warn(msg, "");
        }
    }

    this.changeFunctionCloseRequest = function (index, returnAssetQty) {
        clearErrors();
        var detalleTemp = getIndexDetailOutRequest(index);
        editDetailOutRequest(index, detalleTemp.AssetId, detalleTemp.NameAsset, detalleTemp.StockAsset, detalleTemp.Price, returnAssetQty, detalleTemp.LeftOver);
        return false;
    }


    function editDetailOutRequest(index, assetId, nameAsset, stockAsset, price, returnAssetQty, leftOver) {
        try {
            $.each(outRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    value.AssetId = assetId;
                    value.NameAsset = nameAsset;
                    value.StockAsset = stockAsset;
                    value.Price = price;
                    value.ReturnAssetQty = returnAssetQty;
                    value.LeftOver = leftOver;
                }
            });
            renderDetailAssetList();
            return true;
        }
        catch (exc) { return false; }
    };

    function renderDetailAssetList() {
        abp.ui.setBusy();
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
                    abp.ui.clearBusy();
                }
            }, error: function () { writeError('detalleInventarioAlerts', data.result.message, 'error'); abp.ui.clearBusy(); }
        });
    }


    this.updateRequestsList = function () {
        updateRequestsListLocal();
    };

    function updateRequestsListLocal() {
        abp.ui.setBusy();
        $("#anyPanelBody").html('');
        $.ajax({
            url: searchUrl,
            type: "POST",
            cache: false,
            data: { query: $('#SrchQuery_hidden').val(), page: $('#SrchPage_hidden').val() },
            success: function (data) {
                $("#anyListEntity").html(data);
                abp.ui.clearBusy();
            },
            error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al buscar la Solicitud', 'error'); abp.ui.clearBusy(); }
        });
        return false;
    }
    this.closeRequestPartialAsset = function (assetId, outRequestId) {
        abp.ui.setBusy();
        var temp = "#returnAssetQty" + assetId;
        var returnAssetQty = $(temp).val();
        clearErrors();
        $.ajax({
            url: closeRequestPartialUrl,
            type: "POST",
            cache: false,
            data: { outRequestId: outRequestId, assetId: assetId, qty: returnAssetQty },
            success: function (data) {
                if (data.result!=null) {
                    writeError('closeRequestAlerts', data.result.errorDescription, 'error');
                } else {
                    $("#anyListDetailsEntity").html(data);
                    writeError('closeRequestAlerts', "Movimiento aplicado correctamente", 'success');
                }
                abp.ui.clearBusy();
            },
            error: function (xhr, ajaxOptions, thrownError) { writeError('closeRequestAlerts', 'Error al buscar la Solicitud', 'error'); abp.ui.clearBusy(); }
        });
        return false;
    }
}

ViewOutRequestCloseVariable = new ViewSalidaInventarioProcess();