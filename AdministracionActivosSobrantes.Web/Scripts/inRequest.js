function ViewInRequestProcess() {

    var searchUrl = "/InRequests/AjaxPage";
    var inRequestModel;

    var searchArticuloSolicitudUrl = "/InRequests/SearchArticulos";
    var showArticuloListUrl = "/InRequests/ShowArticuloList";
    var renderDetalleArticuloListUrl = "/InRequests/RenderListDetallesOrden";;
    var addAndRenderDetalleArticuloListUrl = "/InRequests/AddAndRenderListDetallesOrden";

    var addCodeBarUrl = "/InRequests/AddListDetallesOrden";

    var inRequest = function (inRequestId, noRequest, requestDocumentNumber, purchaseOrderNumber, notes, comment, typeInRequest, stateRequest, cellarId, personInCharge, arrayDetalles) {
        this.Id = inRequestId;
        this.NoRequest = noRequest;
        this.RequestDocumentNumber = requestDocumentNumber;
        this.Notes = notes;
        this.Comment = comment;
        this.TypeInRequestValue = typeInRequest;
        this.StateRequest = stateRequest;
        this.CellarId = cellarId;
        this.PurchaseOrderNumber = purchaseOrderNumber;
        this.PersonInCharge = personInCharge;
        this.DetailsRequest = arrayDetalles;
        this.ImagePath1 = "";
        this.ImagePath2 = "";
        this.SignatureData = "";
    };

    var detailInRequest = function (detailId, assetId, nameAsset, stockAsset, price, saved, update, deletep, errorCode, errorDesc, index) {
        this.Index = index;
        this.DetailId = detailId;
        this.AssetId = assetId;
        this.NameAsset = nameAsset;
        this.StockAsset = stockAsset;
        this.Price = price;
        this.Saved = saved; //Si ya esta guardado a a solicitud
        this.Update = update; //Si hay que actualizar algun dato
        this.Delete = deletep; // Si hay que actualizar algun dato
        this.ErrorDescription = errorDesc;
        this.ErrorCode = errorCode;
    };

    this.setSignatureData = function (signature) {
        try { inRequest.SignatureData = signature; }
        catch (exc) { return false; }
        return false;
    };

    this.getSignatureData = function () {
        try { return inRequest.SignatureData; }
        catch (exc) { return false; }
    };

    this.setAssetImagePath1 = function (image) {
        try { inRequest.ImagePath1 = image; }
        catch (exc) { return false; }
        return false;
    };

    this.getAssetImagePath1 = function () {
        try { return inRequest.ImagePath1; }
        catch (exc) { return false; }
    };

    this.setAssetImagePath2 = function (image) {
        try { inRequest.ImagePath2 = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath2 = function () {
        try { return inRequest.ImagePath2; }
        catch (exc) { return false; }
    };
    this.setAssetImagePath3 = function (image) {
        try { inRequest.ImagePath3 = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath3 = function () {
        try { return inRequest.ImagePath3; }
        catch (exc) { return false; }
    };
    this.setAssetImagePath4 = function (image) {
        try { inRequest.ImagePath4 = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath4 = function () {
        try { return inRequest.ImagePath4; }
        catch (exc) { return false; }
    };

    this.addDetailInRequest = function (detailId, assetId, nameAsset, stockAsset, price, saved, update, deletep, errorCode, errorDesc, index) {
        try {
            nameAsset = $("<div>").html(nameAsset).text();
            var requestDetailItem = new detailInRequest(detailId, assetId, nameAsset, stockAsset, price, saved, update, deletep, errorCode, errorDesc, index);
            inRequestModel.DetailsRequest.push(requestDetailItem);
            return requestDetailItem;
        } catch (exc) { return false; }
    };

    this.removeDetailInRequest = function (index) {
        try {
            $.each(inRequestModel.DetailsRequest, function (i, value) {
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

    function getIndexDetailInRequest(index) {
        var detalle;
        try {
            $.each(inRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    detalle = value;
                }
            });
            return detalle;
        }
        catch (exc) { return -1; }
    };

    function editDetailInRequest(index, assetId, assetName, stock, price) {
        try {
            $.each(inRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    value.Update = 1;
                    value.AssetId = assetId;
                    value.NameAsset = assetName;
                    value.StockAsset = stock;
                    value.Price = price;
                    value.Update = 1; // si hay que actualizar algun dato
                }
            });
            renderDetailAssetList();
            return true;
        }
        catch (exc) { return false; }
    };

    this.clearDetailInRequestList = function () {
        try {
            inRequestModel.DetailsRequest = [];
        } catch (exc) { return false; }
    };


    this.setInRequest = function (inRequestId, noRequest, requestDocumentNumber, notes, comment,typeInRequest, stateRequest, cellarId, purchaseOrderNumber, personInCharge) {
        try {
            inRequestModel.Id = inRequestId;
            inRequestModel.NoRequest = noRequest;
            inRequestModel.RequestDocumentNumber = requestDocumentNumber;
            inRequestModel.Notes = notes;
            inRequestModel.Comment = comment;
            inRequestModel.TypeInRequestValue = typeInRequest;
            inRequestModel.StateRequest = stateRequest;
            inRequestModel.CellarId = cellarId;
            inRequestModel.PurchaseOrderNumber = purchaseOrderNumber;
            inRequestModel.PersonInCharge = personInCharge;
            return true;
        } catch (exc) { return false; }
    };

    this.getInRequest = function () {
        try {
            return inRequestModel;
        } catch (exc) {
            return false;
        }
    };

    this.btnShowAssetList = function () {

        if ($('#typeInRequestHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#typeInRequestHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija el tipo de entrada que desea realizar', 'error');
            return false;
        }
        $.ajax({
            url: showArticuloListUrl,
            type: "POST",
            cache: false,
            data: { q: $("#searchBoxArticulo").val(), typeInRequestValue: $('#typeInRequestHidden').val() },
            success: function (data) {
                $('#anyModalForm').html(data);
                //$(this).addClass("done");
                $('#anyModalForm').modal('show');
            }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
        });

        return false;
    };


    this.setInRequestCellarId = function (cellarId) {
        try {
            inRequestModel.CellarId = cellarId;
            $('#cellarIdHidden').val(inRequestModel.CellarId);
        } catch (exc) {
            return false;
        }
    };

    this.setInRequestTypeRequest = function (tmovId) {
        try {

            if (inRequestModel.TypeOutRequestValue != tmovId && inRequestModel.DetailsRequest.length > 0) {
                $.each(inRequestModel.DetailsRequest, function (i, value) {
                    if (value.Delete == 0) {
                        writeError('detalleInventarioAlerts', "Las entradas solo deben ser de una tipo en específico. Por favor si desea cambiar el tipo de entrada, por favor elimine los detalles que pertencen al tipo de entrada anterior.", 'error');
                        $('#TypeInRequest_DD').val(inRequestModel.TypeInRequestValue);
                        return 0;
                    }
                });
            } else {
                inRequestModel.TypeInRequestValue = tmovId;
                $('#typeInRequestHidden').val(inRequestModel.TypeInRequestValue);
            }
        } catch (exc) {
            return false;
        }
    };


    this.initializeInRequest = function () {
        try {
            inRequestModel = new inRequest('', -1, '', '', '', 0, 0, '', '', []);
            return inRequestModel;
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


    function isEmpty(str) { return (!str || 0 === str.length); }

    $('.closeModal').click(function (e) { $('#anyModalForm').modal('hide'); });

    function renderDetailAssetList() {
        clearErrors();
        $.ajax({
            url: renderDetalleArticuloListUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(inRequestModel.DetailsRequest) },
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

    function addAndRenderDetailsAssetList(assetId, nameAsset, stockAsset, price) {
        clearErrors();
        $.ajax({
            url: addAndRenderDetalleArticuloListUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(inRequestModel.DetailsRequest), assetId: assetId, nameAsset: nameAsset, stockAsset: stockAsset, price: price },
            success: function (data) {
                if (data.length == null) {
                    writeError('detalleInventarioAlerts', data.result.message, 'error');
                }
                else {
                    $("#anyListDetailsEntity").html(data);
                    clearDetailsFields();
                    disableDetailsFields();
                }
            }, error: function () { writeError('detalleInventarioAlerts', 'Error al agregar detalle a la solicitud.', 'error'); }
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


    this.assetLoadData = function (assetId, assetName, description, category, price) {
        try {
            $("#idAssetHidden").val(assetId);
            $("#isUpdateHidden").val(0);
            $("#stockText").val(1);
            $("#assetNameText").val(assetName);
            $("#priceText").val(price);
            $('#anyModalForm').modal('hide');
            enableDetailsFields();
        }
        catch (exc) { }
    };

    this.guardarArticulos = function () {
        clearErrors();
        var stock = parseFloat($("#stockText").val());
        var assetId = $("#idAssetHidden").val();

        if (stock < 0) { writeError('detalleInventarioAlerts', 'La cantidad debe ser mayor que 0', 'error'); return false; }
        if (assetId === "" || assetId == null || assetId.length === 0) { writeError('detalleInventarioAlerts', 'Debe seleccionar un activo', 'error'); return false; }

        var isUpdateVal = $("#isUpdateHidden").val();
        var assetName = $("#assetNameText").val();
        var price = $("#priceText").val();
        var index = $("#selectedIndexArticuloHidden").val();

        if (isUpdateVal == 0) { addAndRenderDetailsAssetList(assetId, assetName, stock, price); }
        else { editDetailInRequest(index, assetId, assetName, stock, price); }
        return false;
    };

    this.guardarArticulosCodeBar = function () {
        clearErrors();
        var codeBar = $("#assetNameText").val();
        addCodeBarAsset(codeBar);

        return false;
    };

    function addCodeBarAsset(codeBar) {
        clearErrors();
        $.ajax({
            url: addCodeBarUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(inRequestModel.DetailsRequest), code: codeBar },
            success: function (data) {
                if (data.length == null) {
                    writeError('detalleInventarioAlerts', data.result.message, 'error');
                }
                else {
                    $("#anyListDetailsEntity").html(data);
                    clearDetailsFields();
                    disableDetailsFields();
                }
            }, error: function () { writeError('detalleInventarioAlerts', 'Error al agregar detalle a la solicitud.', 'error'); }
        });
    }

    this.btnEditDetail = function (index) {
        clearErrors();
        var detalleTemp = getIndexDetailInRequest(index);
        $("#isUpdateHidden").val(1);
        $("#selectedIndexArticuloHidden").val(index);
        $("#idAssetHidden").val(detalleTemp.AssetId);
        $("#assetNameText").val(detalleTemp.NameAsset);
        $("#assetNameText").val(detalleTemp.NameAsset);
        parseFloat($("#stockText").val(detalleTemp.StockAsset));
        $("#priceText").val(detalleTemp.Price);
        enableDetailsFields();
        return false;
    };

    this.searchAssets = function () {
        $.ajax({
            url: searchArticuloSolicitudUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(inRequestModel.DetailsRequest), q: $("#searchBoxAsset").val(), cellarId: $('#cellarIdHidden').val(), inRequestId: inRequestModel.Id },
            success: function (data) {
                $('#anyModalForm').html(data);
            }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
        });
        return false;
    };

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

ViewInRequestVariable = new ViewInRequestProcess();