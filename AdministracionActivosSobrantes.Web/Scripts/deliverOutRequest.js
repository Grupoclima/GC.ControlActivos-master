function ViewDeliverProcess() {
    var outRequestModel;
    var renderDetalleArticuloListUrl = "/OutRequests/RenderListDetallesOrden";;

    var addCodeBarUrl = "/OutRequests/RenderListDetallesOrdenCode";

    var renderListAssetIdUrl = "/OutRequests/RenderListDetallesOrdenAssetId";

    var outRequest = function (outRequestId, imagePath1, imagePath2, signatureData, arrayDetalles) {
        this.Id = outRequestId;
        this.DetailsRequest = arrayDetalles;
        this.ImagePath1 = imagePath1;
        this.ImagePath2 = imagePath2;
        this.SignatureData = signatureData;
    };

    var detailOutRequest = function (detailId, assetId,code, nameAsset, stockAsset,price, status, saved, update, deletep, errorCode, errorDesc, index) {
        this.Index = index;
        this.DetailId = detailId;
        this.AssetCode = code;
        this.AssetId = assetId;
        this.NameAsset = nameAsset;
        this.StockAsset = stockAsset;
        this.Price = price;
        this.Status = status;
        this.Saved = saved; //Si ya esta guardado a a solicitud
        this.Update = update; //Si hay que actualizar algun dato
        this.Delete = deletep; // Si hay que actualizar algun dato
        this.ErrorDescription = errorDesc;
        this.ErrorCode = errorCode;
    };

    this.setOutRequest = function (outRequestId,imagePath1, imagePath2, signatureData) {
        try {
            outRequestModel.Id = outRequestId;
            outRequestModel.ImagePath1 = imagePath1;
            outRequestModel.ImagePath2 = imagePath2;
            outRequestModel.SignatureData = signatureData;
            return true;
        } catch (exc) { return false; }
    };

    this.setSignatureData = function (signature) {
        try { outRequest.SignatureData = signature; }
        catch (exc) { return false; }
        return false;
    };

    this.getSignatureData = function () {
        try { return outRequest.SignatureData; }
        catch (exc) { return false; }
    };

    this.setAssetImagePath1 = function (image) {
        try { outRequest.ImagePath1 = image; }
        catch (exc) { return false; }
        return false;
    };

    this.getAssetImagePath1 = function () {
        try { return outRequest.ImagePath1; }
        catch (exc) { return false; }
    };

    this.setAssetImagePath2 = function (image) {
        try { outRequest.ImagePath2 = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath2 = function () {
        try { return outRequest.ImagePath2; }
        catch (exc) { return false; }
    };
    this.setAssetImagePath3 = function (image) {
        try { outRequest.ImagePath3 = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath3 = function () {
        try { return outRequest.ImagePath3; }
        catch (exc) { return false; }
    };
    this.setAssetImagePath4 = function (image) {
        try { outRequest.ImagePath4 = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath4 = function () {
        try { return outRequest.ImagePath4; }
        catch (exc) { return false; }
    };

    this.setSignatureUI = function (signt) {
        try {
            outRequest.SignatureData = signt;
            //$("#signatureRequest").jSignature("setData", "data:" + signt.join(","));
        }
        catch (exc) { return false; }
        return false;
    };
    this.getSignatureUI = function () {
        try { return outRequest.SignatureData; }
        catch (exc) { return false; }
    };

    
    
    this.addDetailOutRequest = function (detailId, assetId, code, nameAsset, stockAsset, price, status, saved, update, deletep, errorCode, errorDesc, index) {
        try {
            nameAsset = $("<div>").html(nameAsset).text();
            var requestDetailItem = new detailOutRequest(detailId, assetId,code, nameAsset, stockAsset, price, status, saved, update, deletep, errorCode, errorDesc, index);
            outRequestModel.DetailsRequest.push(requestDetailItem);
            return requestDetailItem;
        } catch (exc) { return false; }
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

   

    this.clearDetailOutRequestList = function () {
        try {
            outRequestModel.DetailsRequest = [];
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
            outRequestModel = new outRequest('', '','', '', []);
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



    this.addCodeBarAsset=function(codeBar) {
        clearErrors();
        $.ajax({
            url: addCodeBarUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest), code: codeBar},
            success: function (data) {
                if (data.length == null) {
                    writeError('detalleInventarioAlerts', data.result.message, 'error');
                }
                else {
                    $("#anyListDetailsEntity").html(data);
                    clearDetailsFields();
                }
            }, error: function () { writeError('detalleInventarioAlerts', 'Error al agregar detalle a la solicitud.', 'error'); }
        });
    }

    this.deliverAssetId = function (idAsset, impreso) {
        console.log("deliverassetid")
        clearErrors();
        $.ajax({
            url: renderListAssetIdUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest), assetId: idAsset, IMPRESO: impreso},
            success: function (data) {
                if (data.length == null) {
                    writeError('detalleInventarioAlerts', data.result.message, 'error');
                }
                else {
                    $("#anyListDetailsEntity").html(data);
                    clearDetailsFields();
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


    this.guardarArticulosCodeBar = function () {
        clearErrors();
        if ($('#cellarIdHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#cellarIdHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija una Ubicación antes de realizar la búsqueda del Activo', 'error');
            return false;
        }
        if ($('#typeOutRequestHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#typeOutRequestHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija el tipo de salida que desea realizar', 'error');
            return false;
        }
        var codeBar = $("#assetNameText").val();
        addCodeBarAsset(codeBar);
        
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

ViewDeliverOutRequestVariable = new ViewDeliverProcess();