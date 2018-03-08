function ViewSalidaInventarioProcess() {
    var createUrl = "/OutRequests/Create";
    var searchUrl = "/OutRequests/AjaxPage";
    var searchDebounceUrl = "/OutRequests/SearchDebounce";
    var outRequestModel;

    var searchArticuloSolicitudUrl = "/OutRequests/SearchArticulos";
    var showArticuloListUrl = "/OutRequests/ShowArticuloList";
    var renderDetalleArticuloListUrl = "/OutRequests/RenderListDetallesOrden";;
    var addAndRenderDetalleArticuloListUrl = "/OutRequests/AddAndRenderListDetallesOrden";

    var addCodeBarUrl = "/OutRequests/AddListDetallesOrden";

    var outRequest = function (outRequestId, noRequest, requestDocumentNumber, notes, comment, typeOutRequest, stateRequest, cellarId, projectId, dueDate, responsiblePersonId, deliverTo, contractorId, arrayDetalles) {
        this.Id = outRequestId;
        this.NoRequest = noRequest;
        this.RequestDocumentNumber = requestDocumentNumber;
        this.Notes = notes;
        this.Comment = comment;
        this.TypeOutRequestValue = typeOutRequest;
        this.StateRequest = stateRequest;
        this.CellarId = cellarId;
        this.ProjectId = projectId;
        this.ContractorId = contractorId;
        this.AssetsReturnDate = dueDate;
        this.ResponsiblePersonId = responsiblePersonId;
        this.DeliveredTo = deliverTo;
        this.DetailsRequest = arrayDetalles;
        this.ImagePath1 = "";
        this.ImagePath2 = "";
        this.SignatureData = "";
    };

    var detailOutRequest = function (detailId, assetId, nameAsset, stockAsset, price, availability, saved, update, deletep, errorCode, errorDesc, index) {
        this.Index = index;
        this.DetailId = detailId;
        this.AssetId = assetId;
        this.NameAsset = nameAsset;
        this.StockAsset = stockAsset;
        this.Price = price;
        this.Saved = saved; //Si ya esta guardado a a solicitud
        this.Update = update; //Si hay que actualizar algun dato
        this.Delete = deletep; // Si hay que actualizar algun dato
        this.AssetAvailability = availability;
        this.ErrorDescription = errorDesc;
        this.ErrorCode = errorCode;
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

    this.addDetailOutRequest = function (detailId, assetId, nameAsset, stockAsset, price, availability, saved, update, deletep, errorCode, errorDesc, index) {
        try {
            nameAsset = $("<div>").html(nameAsset).text();
            var requestDetailItem = new detailOutRequest(detailId, assetId, nameAsset, stockAsset, price, availability, saved, update, deletep, errorCode, errorDesc, index);
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

    function editDetailOutRequest(index, assetId, assetName, stock, price, availability) {
        try {
            $.each(outRequestModel.DetailsRequest, function (i, value) {
                if (index == value.Index) {
                    value.Update = 1;
                    value.AssetId = assetId;
                    value.NameAsset = assetName;
                    value.StockAsset = stock;
                    value.Price = price;
                    value.AssetAvailability = availability;
                    value.Update = 1; // si hay que actualizar algun dato
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


    this.setOutRequest = function (outRequestId, noRequest, requestDocumentNumber, notes, comment, typeOutRequest, stateRequest, cellarId, projectId, dueDate, responsiblePersonId, deliverTo, contractorId) {
        try {
            outRequestModel.Id = outRequestId;
            outRequestModel.NoRequest = noRequest;
            outRequestModel.RequestDocumentNumber = requestDocumentNumber;
            outRequestModel.Notes = notes;
            outRequestModel.Comment = comment;
            outRequestModel.TypeOutRequestValue = typeOutRequest;
            outRequestModel.StateRequest = stateRequest;
            outRequestModel.CellarId = cellarId;
            outRequestModel.ProjectId = projectId;
            outRequestModel.ContractorId = contractorId;
            outRequestModel.AssetsReturnDate = dueDate;
            outRequestModel.ResponsiblePersonId = responsiblePersonId;
            outRequestModel.DeliveredTo = deliverTo;
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

    this.btnShowAssetList = function () {

        if ($('#cellarIdHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#cellarIdHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija una Ubicación antes de realizar la búsqueda del Activo', 'error');
            return false;
        }
        if ($('#typeOutRequestHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#typeOutRequestHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija el tipo de salida que desea realizar', 'error');
            return false;
        }
        $.ajax({
            url: showArticuloListUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest), q: $("#searchBoxArticulo").val(), cellarId: $('#cellarIdHidden').val(), outRequestId: outRequestModel.Id, typeOutRequestValue: outRequestModel.TypeOutRequestValue },
            success: function (data) {
                $('#anyModalForm').html(data);
                //$(this).addClass("done");
                $('#anyModalForm').modal('show');
            }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
        });

        return false;
    };


    this.setOutRequestCellarId = function (cellarId) {
        try {
            if (outRequestModel.CellarId != cellarId && outRequestModel.DetailsRequest.length > 0) {
                $.each(outRequestModel.DetailsRequest, function (i, value) {
                    if (value.Delete == 0) {
                        writeError('detalleInventarioAlerts', "Las salidas solo deben ser de una Ubicación en específico. Por favor si desea sustituir la Ubicación, por favor elimine los detalles que pertencen a la Ubicación anterior.", 'error');

                        $('#Cellars_DD').val(outRequestModel.CellarId);
                        return 0;
                    }
                });

            } else {
                outRequestModel.CellarId = cellarId;
                $('#cellarIdHidden').val(outRequestModel.CellarId);
            }
        } catch (exc) {
            return false;
        }
    };

    this.setOutRequestTypeRequest = function (tmovId) {
        try {

            if (outRequestModel.TypeOutRequestValue != tmovId && outRequestModel.DetailsRequest.length > 0) {
                $.each(outRequestModel.DetailsRequest, function (i, value) {
                    if (value.Delete == 0) {
                        writeError('detalleInventarioAlerts', "Las salidas solo deben ser de una tipo en específico. Por favor si desea cambiar el tipo de salida, por favor elimine los detalles que pertencen al tipo de salida anterior.", 'error');

                        $('#TypeOutRequest_DD').val(outRequestModel.TypeOutRequestValue);
                        return 0;
                    }
                });

            } else {
                outRequestModel.TypeOutRequestValue = tmovId;
                $('#typeOutRequestHidden').val(outRequestModel.TypeOutRequestValue);
            }
        } catch (exc) {
            return false;
        }
    };

    this.setOutRequestProjectId = function (projectId) {
        try {
            outRequestModel.ProjectId = projectId;
        } catch (exc) {
            return false;
        }
    };

    this.setOutRequestContractorId = function (contractorId) {
        try {
            outRequestModel.ContractorId = contractorId;
        } catch (exc) {
            return false;
        }
    };

    this.setOutRequestResponsiblePersonId = function (responsiblePersonId) {
        try {
            outRequestModel.ResponsiblePersonId = responsiblePersonId;
        } catch (exc) {
            return false;
        }
    };

    this.initializeOutRequest = function () {
        try {
            outRequestModel = new outRequest('', -1, '', '', 0, 0, '', '', '', '', '', '', []);
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

    function addAndRenderDetailsAssetList(assetId, nameAsset, stockAsset, price, availability,number) {
        clearErrors();
        $.ajax({
            url: addAndRenderDetalleArticuloListUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest), assetId: assetId, nameAsset: nameAsset, stockAsset: stockAsset, price: price, availability: availability,number: number },
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


    function addCodeBarAsset(codeBar) {
        clearErrors();
        $.ajax({
            url: addCodeBarUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest), code: codeBar, cellarId: $('#cellarIdHidden').val() },
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


    this.assetLoadData = function (assetId, assetName, description, category, price, availability) {
        try {
            $("#idAssetHidden").val(assetId);
            $("#assetAvailabilityText").val(availability);
            $("#isUpdateHidden").val(0);
            $("#assetNameText").val(assetName);
            $("#priceText").val(price);
            $("#stockText").val("1");
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
        var price = parseFloat($("#priceText").val());
        var availability = parseFloat($("#assetAvailabilityText").val());
        var index = $("#selectedIndexArticuloHidden").val();
        var number = $("#noRequestHidden").val();
       // var requestnumber = $("#RequestNumber").val();
        console.log("entre");
        console.log(number);

        if (isUpdateVal == 0) { addAndRenderDetailsAssetList(assetId, assetName, stock, price, availability,number); }
        else { editDetailOutRequest(index, assetId, assetName, stock, price, availability); }
        return false;
    };

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

    this.btnEditDetail = function (index) {
        clearErrors();
        var detalleTemp = getIndexDetailOutRequest(index);
        $("#isUpdateHidden").val(1);
        $("#selectedIndexArticuloHidden").val(index);
        $("#idAssetHidden").val(detalleTemp.AssetId);
        parseFloat($("#assetAvailabilityText").val(detalleTemp.AssetAvailability));
        $("#assetNameText").val(detalleTemp.NameAsset);
        parseFloat($("#stockText").val(detalleTemp.StockAsset));
        parseFloat($("#priceText").val(detalleTemp.Price));
        enableDetailsFields();
        return false;
    };

    this.searchAssets = function () {
        //alert($('#typeOutRequestValueStockHidden').val());
        //alert($('#pageNumberValueStockHidden').val());
        //alert($('#searchBoxAsset').val());
        //alert($('#cellarIdHidden').val());

        $.ajax({
            url: showArticuloListUrl,
            type: "POST",
            cache: false,
            data: {
                jsonDetallesList: JSON.stringify(outRequestModel.DetailsRequest), q: $("#searchBoxAsset").val(), cellarId: $('#cellarIdHidden').val(),
                outRequestId: outRequestModel.Id,
                typeOutRequestValue: $('#typeOutRequestValueStockHidden').val(),
                page: $('#pageNumberValueStockHidden').val()
            },
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

ViewOutRequestVariable = new ViewSalidaInventarioProcess();