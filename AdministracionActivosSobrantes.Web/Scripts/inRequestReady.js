(function () {

    $(function () {

        var paseInventarioUrl = "/InRequests/ChangeProcessedStatus";
        var deleteUrl = "/InRequests/Delete";
        var searchUrl = "/InRequests/AjaxPage";
        var createUrl = "/InRequests/Create";
        var editUrl = "/InRequests/Edit";

        $('#Cellars_DD').change(function () {
            alert($('#Cellars_DD').val());
            clearErrors();
            $('#cellarIdHidden').val($('#Cellars_DD').val());
            ViewInRequestVariable.setInRequestCellarId($('#Cellars_DD').val());
        });

        $('#TypeInRequest_DD').change(function () {
            //$('#typeInRequestHidden').val($('#TypeInRequest_DD').val());
            ViewInRequestVariable.setInRequestTypeRequest($('#TypeInRequest_DD').val());
        });

        $('#guardarArticuloBtn').click(function (e) {
            ViewInRequestVariable.guardarArticulos();
        });

        $('#btnShowArticulosList').click(function (e) {
            ViewInRequestVariable.btnShowAssetList();
        });

        $('#btnDetalleEdit').click(function (e) {
            ViewInRequestVariable.btnEditDetail();
        });

        $('#btnDeleteDetail').click(function (e) {
            ViewInRequestVariable.btnEditDetail();
        });

        $('#btnDeleteDetalle').click(function (e) {
            clearErrors();
            var index = $(this).attr("data-idrequest");
            ViewInRequestVariable.removeDetailInRequest(index);
            return false;
        });

        $("#assetNameText").keydown(function (e) {
            if (e.keyCode == 13) {
                ViewInRequestVariable.guardarArticulosCodeBar();
            }
        });

        $("#signatureRequest").jSignature({ 'height': '400', 'width': '100%' });

        $('#btnFirmaDigital').click(function (e) {
            $('#signatureModal').modal('show');
            $("#signatureRequest").resize();
            return false;
        });

        $("#resetSignature").click(function (e) { $("#signatureRequest").jSignature("reset"); });

        $("#acceptSignature").click(function (e) {
            var tempSigData = $("#signatureRequest").jSignature("getData", "image");
            //$("#signatureDataHidden").val(tempSigData);
            ViewInRequestVariable.setSignatureData(tempSigData);
            $('#signatureModal').modal('hide');
        });

        //---- Errors Alerts
        function clearErrors() { $('#msgErrorAnyModal').html(''); $('#IndexAlerts').html(''); $('#detalleInventarioAlerts').html(''); }


        $('#SaveRequestData').click(function (e) {
            //if ($(this).find('.input-validation-error').length == 0) {
            //    $(this).find(':submit').attr('disabled', 'disabled');
            //}
            abp.ui.setBusy();
            ViewInRequestVariable.setInRequest($("#inRequestIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
                $("#notasAdicionalesTxt").val(), $("#commentsTxt").val(), $("#typeInRequestHidden").val(), $("#stateRequestHidden").val(), $("#cellarIdHidden").val(), $("#purchaseOrderNumberTxt").val(), $("#personInChargeText").val());

            var dataImage = new FormData();
            var files1 = $("#uploadEditorImage1").get(0).files;
            if (files1.length > 0) {
                dataImage.append("InRequestImagesSection1", files1[0]);
            }
            var files2 = $("#uploadEditorImage2").get(0).files;
            if (files2.length > 0) {
                dataImage.append("InRequestImagesSection2", files2[0]);
            }

            dataImage.append("SignatureStr", ViewInRequestVariable.getSignatureData());

            dataImage.append("InRequestDataSection", JSON.stringify(ViewInRequestVariable.getInRequest()));

            $.ajax({
                url: createUrl,
                type: 'POST',
                data: dataImage,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.result.errorCode == 1) {
                        $("#confirmationModal").modal("show");
                        abp.ui.clearBusy();
                    }
                    if (data.result.errorCode == -1) {
                        //$("#SaveRequestData").removeAttr('disabled', 'disabled');
                        writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
                        abp.ui.clearBusy();
                    }
                }
            }).error(function (error, status) {
                writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
                abp.ui.clearBusy();
            });


            /*$.post($(this).attr('action'), {
                 entity: JSON.stringify(ViewInRequestVariable.getInRequest())
            }, function (data, status) {
                if (data.result.errorCode == 1) {
                    $("#confirmationModal").modal("show");
                    abp.ui.clearBusy();
                }
                if (data.result.errorCode == -1) {
                    $("#SaveRequestData").removeAttr('disabled', 'disabled');
                    writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
                    abp.ui.clearBusy();
                }
            }).error(function (error, status) {
                $(this).find(':submit').removeAttr('disabled');
                $(this).find(':submit').removeAttr('disabled', 'disabled');
                writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
                abp.ui.clearBusy();
            });*/
            return false;
        });

        $('#SaveRequestDataEdit').click(function (e) {
            //if ($(this).find('.input-validation-error').length == 0) {
            //    $(this).find(':submit').attr('disabled', 'disabled');
            //}
            abp.ui.setBusy();
            ViewInRequestVariable.setInRequest($("#inRequestIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
                 $("#notasAdicionalesTxt").val(), $("#commentsTxt").val(), $("#typeInRequestHidden").val(), $("#stateRequestHidden").val(), $("#cellarIdHidden").val(), $("#purchaseOrderNumberTxt").val(), $("#personInChargeText").val());

            var dataImage = new FormData();
            var files1 = $("#uploadEditorImage1").get(0).files;
            if (files1.length > 0) {
                dataImage.append("InRequestImagesSection1", files1[0]);
            }
            var files2 = $("#uploadEditorImage2").get(0).files;
            if (files2.length > 0) {
                dataImage.append("InRequestImagesSection2", files2[0]);
            }

            dataImage.append("InRequestDataSection", JSON.stringify(ViewInRequestVariable.getInRequest()));

            $.ajax({
                url: editUrl,
                type: 'POST',
                data: dataImage,
                contentType: false,
                processData: false,
                success: function (data) {
                    if (data.result.errorCode == 1) {
                        $("#confirmationModal").modal("show");
                        abp.ui.clearBusy();
                    }
                    if (data.result.errorCode == -1) {
                        //$("#SaveRequestData").removeAttr('disabled', 'disabled');
                        writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
                        abp.ui.clearBusy();
                    }
                }
            }).error(function (error, status) {
                writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
                abp.ui.clearBusy();
            });
            return false;
        });
        
        this.deleteFunc = function (btn) {
            clearErrors();
            $('#DeleteConfirmationMain_modal').modal('show');
            $('#ItemToDeleteMain_hidden').val($(btn).attr("data-idrequest"));
            return false;
        }

        this.paseAprovalFunc = function (btn) {
            clearErrors();
            $('#PaseInventarioConfirmationMain_modal').modal('show');
            $('#ItemToInventarioMain_hidden').val($(btn).attr("data-idrequest"));
            return false;
        }

        this.aprovalFunc = function (btn) {
            clearErrors();
            $('#ApprovedConfirmationMain_modal').modal('show');
            $('#ItemToApprovedMain_hidden').val($(btn).attr("data-idrequest"));
            return false;
        }

        this.deliverFunc = function (btn) {
            clearErrors();
            $('#DeliverConfirmationMain_modal').modal('show');
            $('#ItemToDeliverMain_hidden').val($(btn).attr("data-idrequest"));
            return false;
        }

        $('#btnModalOkDeleteMainConfirmation').click(function (e) {
            $('#DeleteConfirmationMain_modal').modal('hide');
            abp.ui.setBusy();

            var id = $('#ItemToDeleteMain_hidden').val();
            $.ajax({
                url: deleteUrl + '/',
                type: "DELETE",
                cache: false,
                data: { id: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 1) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        $('#tableRowId' + id).remove();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'error');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al borrar la solicitud', 'error'); $('body').removeClass("loading"); }
            });

        });


        $('#btnModalOkPaseInventarioConfirmation').click(function (e) {
            $('#PaseInventarioConfirmationMain_modal').modal('hide');
            abp.ui.setBusy();
            var id = $('#ItemToInventarioMain_hidden').val();
            $.ajax({
                url: paseInventarioUrl + '/',
                type: "POST",
                cache: false,
                data: { inRequestId: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        updateRequestsListLocal();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts',  data.result.errorDescription, 'error');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al aplicar la solicitud', 'error'); abp.ui.clearBusy(); }
            });
        });
      

        function updateRequestsListLocal() {
            abp.ui.setBusy();
     
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

        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                //var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                //$("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }
    });
})();
