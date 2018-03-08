(function () {
    $(function () {
        var deleteUrl = "/Asset/Delete";
        var detailUrl = "/Asset/Details";
        var toolKitEnable = "/Asset/EnableToolKit";

        var okFlag = 1;
        var noneFlag = 0;
        var errorFlag = -1;
        //---- Errors Alerts
        function clearErrors() { $("#msgErrorAnyModal").html(""); $("#IndexAlerts").html(""); $("#customFieldsAlerts").html(""); }
        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }
        //-------------------------------------------
        //Assets Events
        $("#Categories_DD").change(function () {
            $("#CategoryIdHidden").val($("#Categories_DD").val());
            ViewAssetVariable.setAssetCategoryId($("#Categories_DD").val());
        });
        $("#AssetsTypes_DD").change(function () {
            $("#AssetType_Hidden").val($("#AssetsTypes_DD").val());
            ViewAssetVariable.setAssetType($("#AssetsTypes_DD").val());
        });

        $('#IsToolKitCb').change(function () {
            if ($(this).is(":checked")) {
                console.log("Activado");
                ViewAssetVariable.setIsAToolInKit(true);
                abp.ui.setBusy();
                $("#anyPanelBody").html("");
                $.ajax({
                    url: toolKitEnable,
                    context: document.body,
                    success: function (data) {
                        $('#anyPanelBody').html(data);
                        $(this).addClass("done");
                        abp.ui.clearBusy();
                    }, error: function (err) { writeError('IndexAlerts', err, 'error'); abp.ui.clearBusy(); }
                });
                return false;
            } else {
                console.log("Desactivado");
                var count = ViewAssetVariable.getAssetToolKitsDetail();
                if (count > 0) {
                    $('#IsToolKitCb').prop('checked', true);
                    ViewAssetVariable.setIsAToolInKit(true);
                    writeError('customFieldsAlerts', 'No Puede quitar el detalle del set porque tiene Activos ingresados', 'error');
                } else {
                    $("#anyPanelBody").html("");
                    ViewAssetVariable.setIsAToolInKit(false);
                }
            }
        });

        $('#CreateAssetForm').submit(function () {
            abp.ui.setBusy();
            ViewAssetVariable.setAssetDto($("#AssetIdHidden").val(), $("#CodeText").val(), $("#NameText").val(), "",
                $("#DescriptionText").val(), $("#AdmissionDateText").val(), $("#DepreciationMonthsQtyText").val(),$('#PriceText').val(),
                $("#IsAToolKitHidden").val(), $("#AssetType_Hidden").val(), $("#CategoryIdHidden").val(), $("#BrandText").val(),
                $("#ModelText").val(), $("#SeriesText").val(), $("#PlateText").val(), $("#CategoryText").val(), $("#ResponsiblePersonText").val());
            console.log($('#PriceText').val());
            var dataImage = new FormData();
            var files = $("#uploadEditorImage").get(0).files;
            if (files.length > 0) {
                dataImage.append("AssetsImagesSection", files[0]);
            }

            dataImage.append("AssetsDataSection", JSON.stringify(ViewAssetVariable.getAssetDto()));
            //var myObj = { entity: JSON.stringify(ViewAssetVariable.getAssetDto())};

            $.ajax({
                url: $(this).attr('action'),
                type: 'POST',
                data: dataImage,
                contentType: false,
                processData: false,
                success: function (data) {
                    $("#createPanelBody").html(data);
                    abp.ui.clearBusy();
                }
            }).error(function (error, status) {
                writeError('IndexAlerts', "Error al Guardar el Activo", 'error');
                abp.ui.clearBusy();
            });

            //$.post($(this).attr('action'), { entity: JSON.stringify(ViewAssetVariable.getAssetDto()) }, function (data, status) {
            //    $("#createPanelBody").html(data);
            //    abp.ui.clearBusy();
            //}).error(function (error, status) {
            //    writeError('IndexAlerts', "Error al Guardar el Activo", 'error');
            //    abp.ui.clearBusy();
            //});
            return false;
        });

        $('#EditAssetForm').submit(function () {
            //if ($(this).find('.input-validation-error').length == 0) {
            //    $(this).find(':submit').attr('disabled', 'disabled');
            //}
            abp.ui.setBusy();
           ViewAssetVariable.setAssetDto($("#AssetIdHidden").val(), $("#CodeText").val(), $("#NameText").val(), "",
                $("#DescriptionText").val(), $("#AdmissionDateText").val(), $("#DepreciationMonthsQtyText").val(),$("#PriceText").val(),
               $("#IsAToolKitHidden").val(), $("#AssetType_Hidden").val(), $("#CategoryIdHidden").val(), $("#BrandText").val(), $("#ModelText").val(),
               $("#SeriesText").val(), $("#PlateText").val(),$("#CategoryText").val(), $("#ResponsiblePersonText").val());
               
            var dataImage = new FormData();
            var files = $("#uploadEditorImage").get(0).files;
            if (files.length > 0) {
                dataImage.append("AssetsImagesSection", files[0]);
            }
            //console.log($("#PriceText").val());

            dataImage.append("AssetsDataSection", JSON.stringify(ViewAssetVariable.getAssetDto()));
            //var myObj = { entity: JSON.stringify(ViewAssetVariable.getAssetDto())};
            console.log("editar")
            $.ajax({
                url: $(this).attr('action'),
                type: 'POST',
                data: dataImage,
                contentType: false,
                processData: false,
                success: function (data) {
                    $("#editPanelBody").html(data);
                    abp.ui.clearBusy();
                }
            }).error(function (error, status) {
                writeError('IndexAlerts', "Error al Guardar el Activo", 'error');
                abp.ui.clearBusy();
            });

            /*$.post($(this).attr('action'), { entity: JSON.stringify(ViewAssetVariable.getAssetDto()) }, function (data, status) {
                $("#editPanelBody").html(data);
                abp.ui.clearBusy();
            }).error(function (error, status) {
                writeError('IndexAlerts', "Error al Guardar el Activo", 'error');
                abp.ui.clearBusy();
            });*/
            return false;
        });
        this.detailFunc = function (btn) {
            clearErrors();
            if (!ViewAssetVariable.isEmptyPublic(detailUrl)) {
                var id = btn.attr("data-idEntity");
                var url = detailUrl + "/" + id;
                ViewAssetVariable.getRequest(url);
            }
            return false;
        }
        this.deleteFunc = function (btn) {
            clearErrors();
            $("#DeleteConfirmation_modal").modal("show");
            $("#ItemToDelete_hidden").val(btn.attr("data-idEntity"));
            return false;
        }

        this.showAssetBtnlist = function (btn) {
            ViewAssetVariable.btnShowAssetList();
            return false;
        }
        //Asset Delete Modal
        $("#btnModalOkDeleteConfirmation").click(function (e) {
            $("#DeleteConfirmation_modal").modal("hide");
            abp.ui.setBusy();
            if (!ViewAssetVariable.isEmptyPublic(deleteUrl)) {
                var id = $("#ItemToDelete_hidden").val();
                $.ajax({
                    url: deleteUrl + "/",
                    type: "DELETE",
                    cache: false,
                    data: { id: id },
                    success: function (data) {
                        if (data === okFlag) {
                            abp.notify.success("Activo eliminado correctamente", "");
                            ViewAssetVariable.refreshList();
                            abp.ui.clearBusy();
                        } else if (data < noneFlag) {
                            writeError("IndexAlerts", "Error al eliminar el Activo", "error");
                            abp.ui.clearBusy();
                        }
                    },
                    error: function (xhr, ajaxOptions, thrownError) {
                        writeError("IndexAlerts", "¡Error al borrar el activo!", "error");
                        abp.ui.clearBusy();
                    }
                });
            }
            return false;
        });
        //-------------------------------------------
        //Custom Fields Events
        $("#btnSaveCustomField").click(function (e) {
            var isUpdateVal = $("#isUpdateCustomFieldHidden").val();
            if (parseInt(isUpdateVal) === 0) {
                $("#isUpdateCustomFieldHidden").val(0);
                $("#selectedCustomFieldIndexHidden").val(-1);
                $("#idCustomFieldHidden").val(-1);
                $("#customFieldValueType").val($("#CustomFieldTypes_DD").val());
            }
            ViewAssetVariable.btnSaveCustomField();
        });

        $("#CustomFieldTypes_DD").change(function () {
            var typeValue = $("#CustomFieldTypes_DD").val();
            if (parseInt(typeValue) === 0) { $("#customFieldsValueText").prop("type", "text"); }
            else if (parseInt(typeValue) === 1) { $("#customFieldsValueText").prop("type", "date"); }
            else if (parseInt(typeValue) === 2) { $("#customFieldsValueText").prop("type", "number"); }
            else if (parseInt(typeValue) === 3) { $("#customFieldsValueText").prop("type", "number"); }
            $("#customFieldValueType").val($("#CustomFieldTypes_DD").val());
        });

        //$("#btnEditCustomField").click(function (e) {
        //    clearErrors();
        //    var index = $(this).attr("data-idCustomField");
        //   ViewAssetVariable.btnEditCustomField(index);
        //    return false;
        //});

        //$('#btnDeleteCustomField').click(function (e) {
        //    clearErrors();
        //    var index = $(this).attr("data-idCustomField");
        //    ViewAssetVariable.removeAssetCustomField(index);
        //    return false;
        //});
        //-------------------------------------------

    });
})();
