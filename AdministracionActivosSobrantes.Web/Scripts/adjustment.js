function ViewAdjustmentProcess() {

    var searchUrl = "/Adjustments/AjaxPage";
    var adjustmentModel;

    var searchArticuloSolicitudUrl = "/Adjustments/SearchArticulos";
    var showArticuloListUrl = "/Adjustments/ShowArticuloList";
    var renderDetalleArticuloListUrl = "/Adjustments/RenderListDetallesOrden";;
    var addAndRenderDetalleArticuloListUrl = "/Adjustments/AddAndRenderListDetallesOrden";

    var outRequest = function (id, noRequest, requestDocumentNumber, notes, typeAdjustment, cellarId, personInCharge, arrayDetalles) {
        this.Id = id;
        this.NoRequest = noRequest;
        this.RequestDocumentNumber = requestDocumentNumber;
        this.Notes = notes;
        this.TypeAdjustmentValue = typeAdjustment;
        this.CellarId = cellarId;
        this.PersonInCharge = personInCharge;
        this.DetailsAdjustment = arrayDetalles;
    };

    var detailAdjustment = function (detailId, assetId, nameAsset, stockAsset, price, saved, update, deletep, errorCode, errorDesc, index) {
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

    this.addDetailAdjustment = function (detailId, assetId, nameAsset, stockAsset, price, saved, update, deletep, errorCode, errorDesc, index) {
        try {
            nameAsset = $("<div>").html(nameAsset).text();
            var requestDetailItem = new detailAdjustment(detailId, assetId, nameAsset, stockAsset, price, saved, update, deletep, errorCode, errorDesc, index);
            adjustmentModel.DetailsAdjustment.push(requestDetailItem);
            return requestDetailItem;
        } catch (exc) { return false; }
    };

    this.removeDetailAdjustment = function (index) {
        try {
            $.each(adjustmentModel.DetailsAdjustment, function (i, value) {
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

    function getIndexDetailAdjustment(index) {
        var detalle;
        try {
            $.each(adjustmentModel.DetailsAdjustment, function (i, value) {
                if (index == value.Index) {
                    detalle = value;
                }
            });
            return detalle;
        }
        catch (exc) { return -1; }
    };

    function editDetailAdjustment(index, assetId, assetName, stock, price) {
        try {
            $.each(adjustmentModel.DetailsAdjustment, function (i, value) {
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

    this.clearDetailAdjustmentList = function () {
        try {
            adjustmentModel.DetailsAdjustment = [];
        } catch (exc) { return false; }
    };


    this.setAdjustment = function (id, noRequest, requestDocumentNumber, notes, typeOutRequest, cellarId, personInCharge) {
        try {
            adjustmentModel.Id = id;
            adjustmentModel.NoRequest = noRequest;
            adjustmentModel.RequestDocumentNumber = requestDocumentNumber;
            adjustmentModel.Notes = notes;
            adjustmentModel.TypeAdjustmentValue = typeOutRequest;
            adjustmentModel.CellarId = cellarId;
            adjustmentModel.PersonInCharge = personInCharge;
            return true;
        } catch (exc) { return false; }
    };

    this.getAdjustment = function () {
        try {
            return adjustmentModel;
        } catch (exc) {
            return false;
        }
    };

    this.btnShowAssetList = function () {

        if ($('#cellarIdHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#cellarIdHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija una Ubicación antes de realizar la búsqueda del Activo', 'error');
            return false;
        }
        if ($('#typeAdjustmentHidden').val() == '00000000-0000-0000-0000-000000000000' || $('#typeAdjustmentHidden').val() == "") {
            writeError('detalleInventarioAlerts', 'Por favor elija el tipo de ajuste que desea realizar', 'error');
            return false;
        }
        $.ajax({
            url: showArticuloListUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(adjustmentModel.DetailsAdjustment), q: $("#searchBoxArticulo").val(), cellarId: $('#cellarIdHidden').val(), typeAdjustment: $('#typeAdjustmentHidden').val() },
            success: function (data) {
                $('#anyModalForm').html(data);
                //$(this).addClass("done");
                $('#anyModalForm').modal('show');
            }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
        });

        return false;
    };


    this.setAdjustmentCellarId = function (cellarId) {
        try {
            if (adjustmentModel.CellarId != cellarId && adjustmentModel.DetailsAdjustment.length > 0) {
                $.each(adjustmentModel.DetailsAdjustment, function (i, value) {
                    if (value.Delete == 0) {
                        writeError('detalleInventarioAlerts', "Las salidas solo deben ser de una Ubicación en específico. Por favor si desea sustituir la Ubicación, por favor elimine los detalles que pertencen a la Ubicación anterior.", 'error');

                        $('#Cellars_DD').val(adjustmentModel.CellarId);
                        return 0;
                    }
                });

            } else {
                adjustmentModel.CellarId = cellarId;
                $('#cellarIdHidden').val(adjustmentModel.CellarId);
            }
        } catch (exc) {
            return false;
        }
    };

    this.setAdjustmentTypeRequest = function (tmovId) {
        try {
            if (adjustmentModel.TypeAdjustmentValue != tmovId && adjustmentModel.DetailsAdjustment.length > 0) {
                $.each(adjustmentModel.DetailsAdjustment, function (i, value) {
                    if (value.Delete == 0) {
                        writeError('detalleInventarioAlerts', "Las salidas solo deben ser de una Ubicación en específico. Por favor si desea sustituir la Ubicación, por favor elimine los detalles que pertencen a la Ubicación anterior.", 'error');

                        $('#TypeAdjustment_DD').val(adjustmentModel.TypeAdjustmentValue);
                        return 0;
                    }
                });

            } else {
                adjustmentModel.TypeAdjustmentValue = tmovId;
                $('#typeAdjustmentHidden').val($('#TypeAdjustment_DD').val());
            }
        } catch (exc) {
            return false;
        }
    };

    this.initializeAdjustment = function () {
        try {
            adjustmentModel = new outRequest('', -1, '', '', 0, '', '', []);
            return adjustmentModel;
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
            data: { jsonDetallesList: JSON.stringify(adjustmentModel.DetailsAdjustment) },
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
            data: { jsonDetallesList: JSON.stringify(adjustmentModel.DetailsAdjustment), assetId: assetId, nameAsset: nameAsset, stockAsset: stockAsset, price: price },
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
            $("#stockText").val(1);
            $("#assetNameText").val(assetName);
            $("#priceText").val(price);
            $('#anyModalForm').modal('hide');
            enableDetailsFields();
        }
        catch (exc) { }
    };

    this.saveAssets = function () {
        clearErrors();
        var stock = parseFloat($("#stockText").val());
        var assetId = $("#idAssetHidden").val();

        if (stock < 0) { writeError('detalleInventarioAlerts', 'La cantidad debe ser mayor que 0', 'error'); return false; }
        if (assetId === "" || assetId == null || assetId.length === 0) { writeError('detalleInventarioAlerts', 'Debe seleccionar un activo', 'error'); return false; }

        var isUpdateVal = $("#isUpdateHidden").val();
        var assetName = $("#assetNameText").val();
        var price = parseFloat($("#priceText").val());
        var index = $("#selectedIndexArticuloHidden").val();

        if (isUpdateVal == 0) { addAndRenderDetailsAssetList(assetId, assetName, stock, price); }
        else { editDetailAdjustment(index, assetId, assetName, stock, price); }
        return false;
    };


    this.btnEditDetail = function (index) {
        clearErrors();
        var detalleTemp = getIndexDetailAdjustment(index);
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
        $.ajax({
            url: searchArticuloSolicitudUrl,
            type: "POST",
            cache: false,
            data: { jsonDetallesList: JSON.stringify(adjustmentModel.DetailsAdjustment), q: $("#searchBoxAsset").val(), cellarId: $('#cellarIdHidden').val(), outRequestId: adjustmentModel.Id },
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

ViewAdjustmentVariable = new ViewAdjustmentProcess();