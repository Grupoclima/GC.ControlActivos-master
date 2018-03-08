function ViewAssetProcess() {
    //-----------Asset Variables
    var searchUrl = "/Asset/AjaxPage";
    var searchDebounceUrl = "/Asset/SearchDebounce";
    var searchArticuloSolicitudUrl = "/Asset/SearchArticulos";
    var showArticuloListUrl = "/Asset/ShowArticuloList";
    var okFlag = 1;
    var noneFlag = 0;
    var errorFlag = -1;
    var assetModelDto;
    //----------------------------------------------------------------------------
    //------Generic Functions-----------------------------------------------------
    function writeError(control, msg, type) {
        if (type === "success") { abp.notify.success(msg, ""); }
        else if (type === "error") {
            abp.notify.error(msg, "");
            var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
            $("#" + control).html(alert);
        } else { abp.notify.warn(msg, ""); }
    }
    function clearErrors() {
        $("#msgErrorAnyModal").html("");
        $("#IndexAlerts").html("");
    }
    this.getRequest = function (url) {
        clearErrors();
        abp.ui.setBusy();
        $.ajax({
            url: url,
            context: document.body,
            success: function (data) {
                $(".modal-body p.body").html(data);
                $("#anyModalForm").modal("show");
                abp.ui.clearBusy();
            },
            error: function (err) {
                writeError("IndexAlerts", "¡Error al consultar los datos del Activo!", "error");
                abp.ui.clearBusy();
            }
        });
    }
    function isEmpty(str) { return (!str || 0 === str.length); }
    this.isEmptyPublic = function (str) { return isEmpty(str); }
    $(".closeModal").click(function (e) { $("#anyModalForm").modal("hide"); });

    this.btnShowAssetList = function () {

        $.ajax({
            url: showArticuloListUrl,
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
    };

    //---------------------------------------------------------------------------------
    //--- Asset Butons Events Functions------------------------------------------------
    //Get and refresh list of entities from server
    this.refreshList = function () {
        $.ajax({
            url: searchUrl,
            type: "GET",
            cache: false,
            data: { query: $("#SrchQuery_hidden").val(), page: $("#SrchPage_hidden").val() },
            success: function (data) {
                $("#anyListEntity").html(data);
            },
            error: function (xhr, ajaxOptions, thrownError) {
                writeError("IndexAlerts", "¡Error al consultar los Activos!", "error");
            }
        });
        return false;
    };

    // Main Search, typing timer debouce
    this.searchDebounce = function () {
        $.ajax({
            url: searchDebounceUrl,
            type: "POST",
            cache: false,
            data: { query: $("#SearchAssetTextInput").val() },
            success: function (data) {
                $("#anyListEntity").html(data);
            }, error: function (err) { writeError("IndexAlerts", "¡Error al consultar los Activos!", "error"); }
        });
        return false;
    };

    //---------------------------------------------------------------
    //--------Asset Entities, Logic Dto
    var assetEntity = function (assetId, code, name, barcode, description, admissionDate, monthDepreciation, price, isAToolInKit, assetType, categoryId, arrayCustomFields,brand, modelstr, series, plate, arrayToolKits, categoryStr, responsiblePersonStr) {
        this.Id = assetId;
        this.Code = code;
        this.Name = name;
        this.Barcode = barcode;
        this.Description = description;
        this.AdmissionDate = admissionDate;
        this.DepreciationMonthsQty = monthDepreciation;
        this.Price = price;
        this.IsAToolInKit = isAToolInKit;
        this.AssetType = assetType;
        this.CategoryId = categoryId;
        this.ImagePath = "";
        this.Brand = brand;
        this.Modelstr = modelstr;
        this.Series = series;
        this.Plate = plate;
        this.CategoryStr = categoryStr;
        this.ResponsiblePersonStr = responsiblePersonStr;
        //this.ToolKit = toolKit;
        this.CustomFieldsDto = arrayCustomFields;
        this.DetailAssetToolKitsDto = arrayToolKits;
    };
    this.setAssetDto = function (assetId, code, name, barcode, description, admissionDate, depreciationMonthsQty, price, isAToolInKit, assetType, categoryId, brand, modelstr, series, plate,categoryStr, responsiblePersonStr) {
        try {
            assetModelDto.Id = assetId;
            assetModelDto.Code = code;
            assetModelDto.Name = name;
            assetModelDto.Barcode = barcode;
            assetModelDto.Description = description;
            assetModelDto.AdmissionDate = admissionDate;
            assetModelDto.DepreciationMonthsQty = depreciationMonthsQty;
            assetModelDto.Price = price;
            assetModelDto.IsAToolInKit = isAToolInKit;
            assetModelDto.AssetType = assetType;
            assetModelDto.CategoryId = categoryId;
            assetModelDto.Brand = brand;
            assetModelDto.Modelstr = modelstr;
            assetModelDto.Series = series;
            assetModelDto.Plate = plate;
            assetModelDto.CategoryStr = categoryStr;
            assetModelDto.ResponsiblePersonStr = responsiblePersonStr;
            return true;
        } catch (exc) { return false; }
    };

    this.setAssetImagePath = function (image) {
        try { assetModelDto.ImagePath = image; }
        catch (exc) { return false; }
        return false;
    };
    this.getAssetImagePath = function () {
        try { return assetModelDto.ImagePath; }
        catch (exc) { return false; }
    };

    this.setIsAToolInKit = function (value) {
        try {
            assetModelDto.IsAToolInKit = value;
            $('#IsAToolKitHidden').val(assetModelDto.IsAToolInKit);

        } catch (exc) {
            return false;
        }
    };

    this.initializeAssetDto = function () {
        try {
            assetModelDto = new assetEntity('', '', '', '', '', '', '', 0, 'false', 0, '', [], '', '', '', '', []);
            return assetModelDto;
        } catch (exc) {
            return false;
        }
    };
    this.getAssetDto = function () {
        try { return assetModelDto; }
        catch (exc) { return false; }
    };
    this.setAssetCategoryId = function (categoryId) {
        try { assetModelDto.ProjectId = categoryId; }
        catch (exc) { return false; }
        return false;
    };
    this.setAssetType = function (assetType) {
        try { assetModelDto.AssetType = assetType; }
        catch (exc) { return false; }
        return false;
    };
    //--------------------------------------------------------------------------------------------------------------
    //--------CUSTOM Fields Entities, Logic Dto-----------------------------------------------------------------------------------------
    //-----------Custom Fiels Variables
    var renderCustomFieldsListUrl = "/Asset/RenderListCustomFields";;
    var addAndRenderCustomFieldsListUrl = "/Asset/AddAndRenderListCustomFields";
    var assetCustomFieldEntity = function (customFieldId, assetId, name, value, customFieldType, saved, update, deleteCf, errorCode, errorDesc, index) {
        this.Index = index;
        this.Id = customFieldId;
        this.AssetId = assetId;
        this.Name = name;
        this.Value = value;
        this.CustomFieldType = customFieldType;
        this.Saved = saved; //Si ya esta guardado a a solicitud
        this.Update = update; //Si hay que actualizar algun dato
        this.Delete = deleteCf; // Si hay que actualizar algun dato
        this.ErrorDescription = errorDesc;
        this.ErrorCode = errorCode;
    };
    this.addCustomFieldToAsset = function (customFieldId, assetId, name, value, customFieldType, saved, update, deleteCf, errorCode, errorDesc, index) {
        try {
            name = $("<div>").html(name).text();
            var newCustomField = new assetCustomFieldEntity(customFieldId, assetId, name, value, customFieldType, saved, update, deleteCf, errorCode, errorDesc, index);
            assetModelDto.CustomFieldsDto.push(newCustomField);
            return newCustomField;
        } catch (exc) {
            return false;
        }
    };
    function clearCustomFields() {
        $(".clearCustomFields").val('');
        return true;
    }
    function enablecustomFieldsBoxes() {
        $(".customFieldsAddPanel").prop('disabled', false);
        return true;
    }
    function disablecustomFieldsBoxes() {
        $(".customFieldsAddPanel").prop('disabled', true);
        return true;
    }
    this.assetLoadData = function (customFieldId, customFieldName, customFieldType, value) {
        try {
            $("#idCustomFieldHidden").val(customFieldId);
            $("#isUpdateCustomFieldHidden").val(0);
            $("#customFieldNameText").val(customFieldName);
            $("#customFieldsValueText").val(value);
            //CustomFieldTypes_DD
            //$('#anyModalForm').modal('hide');
            enablecustomFieldsBoxes();
        } catch (exc) { }
    };
    function renderCustomFieldList() {
        clearErrors();
        $.ajax({
            url: renderCustomFieldsListUrl,
            type: "POST",
            cache: false,
            data: { jsonCustomFieldsList: JSON.stringify(assetModelDto.CustomFieldsDto) },
            success: function (data) {
                if (data.Error === -1) {
                    writeError('customFieldsAlerts', data.Message, 'error');
                } else {
                    $("#anyListCustomFieldsEntity").html(data);
                    clearCustomFields();
                    disablecustomFieldsBoxes();
                    $("#isUpdateCustomFieldHidden").val(0);
                }
            },
            error: function () { writeError('customFieldsAlerts', data.Message, 'error'); }
        });
    }
    function addAndRenderAssetCustomFieldsList(assetId, name, value, customFieldType) {
        clearErrors();
        $.ajax({
            url: addAndRenderCustomFieldsListUrl,
            type: "POST",
            cache: false,
            data: { jsonCustomFieldsList: JSON.stringify(assetModelDto.CustomFieldsDto), assetId: assetModelDto.Id, name: name, value: value, type: customFieldType },
            success: function (data) {
                if (data.Error === -1) {
                    writeError('customFieldsAlerts', data.Message, 'error');
                } else {
                    $("#anyListCustomFieldsEntity").html(data);
                    clearCustomFields();
                    disablecustomFieldsBoxes();
                    $("#isUpdateCustomFieldHidden").val(0);
                }
            },
            error: function () { writeError('customFieldsAlerts', 'Error al agregar detalle a la solicitud.', 'error'); }
        });
    }
    function editAssetCustomField(index, customFieldId, name, valueTemp, customFieldType) {
        try {
            $.each(assetModelDto.CustomFieldsDto, function (i, value) {
                if (parseInt(index) === value.Index) {
                    value.Update = 1;
                    value.Id = customFieldId;
                    value.Name = name;
                    value.Value = valueTemp;
                    value.CustomFieldType = customFieldType;
                    value.Update = 1; // si hay que actualizar algun dato
                }
            });
            renderCustomFieldList();
            return true;
        } catch (exc) {
            return false;
        }
    };

    //To Save New Custom Field
    this.btnSaveCustomField = function () {
        clearErrors();
        var customFieldId = $("#idCustomFieldHidden").val();
        var isUpdateVal = $("#isUpdateCustomFieldHidden").val();
        var cfName = $("#customFieldNameText").val();
        var value = $("#customFieldsValueText").val();
        var index = $("#selectedCustomFieldIndexHidden").val();
        var customFieldType = parseInt($("#customFieldValueType").val());
        if (parseInt(isUpdateVal) === 0) { addAndRenderAssetCustomFieldsList(customFieldId, cfName, value, customFieldType); }
        else { editAssetCustomField(index, customFieldId, cfName, value, customFieldType); }
        return false;
    };


    //Remove Custom Field From List
    this.removeAssetCustomField = function (btn) {
        try {
            clearErrors();
            var index = btn.attr("data-idCustomField");

            $.each(assetModelDto.CustomFieldsDto, function (i, value) {
                if (parseInt(index) === value.Index) {
                    value.Delete = 1;
                    value.Update = 1;
                }
            });
            renderCustomFieldList();
            return true;
        } catch (exc) { return false; }
    };

    function getAssetCustomFieldAt(index) {
        var cf;
        try {
            $.each(assetModelDto.CustomFieldsDto, function (i, value) {
                if (parseInt(index) === value.Index) {
                    cf = value;
                }
            });
            return cf;
        } catch (exc) { return -1; }
    };

    //Edit the Custom field of List
    this.btnEditCustomField = function (btn) {
        clearErrors();
        var index = btn.attr("data-idCustomField");
        var cfv = getAssetCustomFieldAt(index);
        $("#isUpdateCustomFieldHidden").val(1);
        $("#idCustomFieldHidden").val(cfv.Id);
        $("#selectedCustomFieldIndexHidden").val(index);
        $("#customFieldsValueText").val(cfv.Value);
        $("#customFieldNameText").val(cfv.Name);
        $("#customFieldValueType").val(cfv.CustomFieldType);
        $("#CustomFieldTypes_DD").val(cfv.CustomFieldType).change();
        return false;
    };
    this.clearCustomFieldList = function () {
        try {
            assetModelDto.CustomFieldsDto = [];
        } catch (exc) { return false; }
    };
    //--------END CUSTOM FIELDS LOGIC-----------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------

    //--------------------------------------------------------------------------------------------------------------
    //--------TOOLKits Entities, Logic Dto-----------------------------------------------------------------------------------------
    //-----------ToolKits Variables
    var renderToolKitsListUrl = "/Asset/RenderListToolKits";;
    var addAndRenderToolKitsListUrl = "/Asset/AddAndRenderListToolKits";
    var assetToolKitsEntity = function (toolKitdId, assetId, name, quatity, code, saved, update, deleteCf, errorCode, errorDesc, index) {
        this.Index = index;
        this.Id = toolKitdId;
        this.AssetId = assetId;
        this.Name = name;
        this.Quatity = quatity;
        this.Code = code;
        this.Saved = saved; //Si ya esta guardado a a solicitud
        this.Update = update; //Si hay que actualizar algun dato
        this.Delete = deleteCf; // Si hay que actualizar algun dato
        this.ErrorDescription = errorDesc;
        this.ErrorCode = errorCode;
    };
    this.addToolKitsToAsset = function (toolKitdId, assetId, name, quatity, code, saved, update, deleteCf, errorCode, errorDesc, index) {
        try {
            name = $("<div>").html(name).text();
            var newtoolKit = new assetToolKitsEntity(toolKitdId, assetId, name, quatity, code, saved, update, deleteCf, errorCode, errorDesc, index);
            assetModelDto.DetailAssetToolKitsDto.push(newtoolKit);
            return newtoolKit;
        } catch (exc) {
            return false;
        }
    };
    function clearToolKitsFields() {
        $(".clearToolKits").val('');
        return true;
    }
    function enableToolKitsBoxes() {
        $(".toolKitsAddPanel").prop('disabled', false);
        return true;
    }
    function disableToolKitsBoxes() {
        $(".toolKitsAddPanel").prop('disabled', true);
        return true;
    }

    this.assetLoadDataToolKit = function (assetId, name, description, code) {
        try {
            $("#assetIdHiddenToolKit").val(assetId);
            $("#isUpdateHiddenToolKit").val(0);
            $("#nameTextToolKit").val(name);
            //$("#quatityText").val(quatity);
            $("#assetCodeHiddenToolKit").val(code);
            //CustomFieldTypes_DD
            //$('#anyModalForm').modal('hide');
            enableToolKitsBoxes();
        } catch (exc) { }
    };

    //To Save New Custom Field
    this.btnSaveToolKit = function () {
        clearErrors();
        var assetId = $("#assetIdHiddenToolKit").val();
        var isUpdateVal = $("#isUpdateHiddenToolKit").val();
        var name = $("#nameTextToolKit").val();
        var quatity = $("#quatityText").val();
        var code = $("#assetCodeHiddenToolKit").val();
        var index = $("#selectedCustomFieldIndexHidden").val();
        if (parseInt(isUpdateVal) === 0) { addAndRenderAssetToolKitsList(assetId, name, code, quatity); }
        else { editAssetCustomField(index, customFieldId, cfName, value, customFieldType); }
        return false;
    };

    function renderToolKitsList() {
        clearErrors();
        $.ajax({
            url: renderToolKitsListUrl,
            type: "POST",
            cache: false,
            data: { jsonToolKitsList: JSON.stringify(assetModelDto.DetailAssetToolKitsDto) },
            success: function (data) {
                if (data.Error === -1) {
                    writeError('customFieldsAlerts', data.Message, 'error');
                } else {
                    $("#renderTableToolKits").html(data);
                    clearToolKitsFields();
                    disableToolKitsBoxes();
                    $("#isUpdateCustomFieldHidden").val(0);
                }
            },
            error: function () { writeError('customFieldsAlerts', data.Message, 'error'); }
        });
    }
    function addAndRenderAssetToolKitsList(assetId, name, code, quatity) {
        clearErrors();
        $.ajax({
            url: addAndRenderToolKitsListUrl,
            type: "POST",
            cache: false,
            data: { jsonToolKitsList: JSON.stringify(assetModelDto.DetailAssetToolKitsDto), assetId: assetId, name: name, code: code, quatity: quatity },
            success: function (data) {
                if (data.Error === -1) {
                    writeError('customFieldsAlerts', data.Message, 'error');
                } else {
                    $("#renderTableToolKits").html(data);
                    clearToolKitsFields();
                    disableToolKitsBoxes();
                    $("#isUpdateCustomFieldHidden").val(0);
                }
            },
            error: function () { writeError('customFieldsAlerts', 'Error al agregar detalle a la solicitud.', 'error'); }
        });
    }


    //Remove Custom Field From List
    this.removeAssetToolKits = function (index) {
        try {
            clearErrors();
            $.each(assetModelDto.DetailAssetToolKitsDto, function (i, value) {
                if (parseInt(index) === value.Index) {
                    value.Delete = 1;
                    value.Update = 1;
                }
            });
            renderToolKitsList();
            return true;
        } catch (exc) { return false; }
    };

    function getAssetToolKitsAt(index) {
        var cf;
        try {
            $.each(assetModelDto.DetailAssetToolKitsDto, function (i, value) {
                if (parseInt(index) === value.Index) {
                    cf = value;
                }
            });
            return cf;
        } catch (exc) { return -1; }
    };

    this.getAssetToolKitsDetail = function () {
        var cf = 0;
        try {
            $.each(assetModelDto.DetailAssetToolKitsDto, function (i, value) {
                if (value.Delete != 1) {
                    cf = cf + 1;
                }
              });
            return cf;
        } catch (exc) { return -1; }
    };

    this.clearToolKitsList = function () {
        try {
            assetModelDto.DetailAssetToolKitsDto = [];
        } catch (exc) { return false; }
    };
    //--------END ToolKits LOGIC-----------------------------------------------------------------------------------
    //--------------------------------------------------------------------------------------------------------------
}
ViewAssetVariable = new ViewAssetProcess();