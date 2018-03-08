(function () {

    $(function () {

        var showCloseRequest = "/OutRequests/CloseRequest";
        var paseInventarioUrl = "/OutRequests/WaitApprovalStatus";
        var approvedUrl = "/OutRequests/ApprovedStatus";
        var deliverUrl = "/OutRequests/DeliverStatus";
        var deleteUrl = "/OutRequests/Delete";
        var searchUrl = "/OutRequests/AjaxPage";
        var appyCloseRequests = "/OutRequests/CloseRequestApply";
        var disprovedUrl = "/OutRequests/DisprovedStatus";
        var editDetailUrl = "/OutRequests/EditDetail";
        var editDetailPostUrl = "/OutRequests/EditDetailAndChangeStatus";
        var searchDebounceProjectUrl = "/OutRequests/SearchDebounceProject";
        var createRequest = "/OutRequests/Create";
        var editRequest = "/OutRequests/Edit";
        var deliverRequestUrl = "/OutRequests/DeliverOutRequestAction";
        var editarimagenes = "/OutRequests/EditPictures"

        var showProjectListUrl = "/OutRequests/ShowProjectList";

        $('#Cellars_DD').change(function () {
            //$('#cellarIdHidden').val($('#Bodegas_DD').val());
            clearErrors();
            ViewOutRequestVariable.setOutRequestCellarId($('#Cellars_DD').val());
        });

        $('#TypeOutRequest_DD').change(function () {
            //$('#typeOutRequestHidden').val($('#TypeOutRequest_DD').val());
            ViewOutRequestVariable.setOutRequestTypeRequest($('#TypeOutRequest_DD').val());
        });

        $('#Projects_DD').change(function () {
            $('#projectIdHidden').val($('#Projects_DD').val());
            ViewOutRequestVariable.setOutRequestProjectId($('#Projects_DD').val());
        });

        $('#Contractors_DD').change(function () {
            $('#contractorIdHidden').val($('#Contractors_DD').val());
            ViewOutRequestVariable.setOutRequestContractorId($('#Contractors_DD').val());
        });

        $('#Responsibles_DD').change(function () {
            $('#responsiblePersonHidden').val($('#Responsibles_DD').val());
            ViewOutRequestVariable.setOutRequestResponsiblePersonId($('#Responsibles_DD').val());
        });

        $('#guardarArticuloBtn').click(function (e) {
            ViewOutRequestVariable.guardarArticulos();
        });

        $('#btnShowArticulosList').click(function (e) {
            ViewOutRequestVariable.btnShowAssetList();
        });

        $('#btnShowProjectList').click(function (e) {
            var test = 0;
            $.ajax({
                url: showProjectListUrl,
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
        });

        $('#btnDetalleEdit').click(function (e) {
            ViewOutRequestVariable.btnEditDetail();
        });

        $('#btnDeleteDetail').click(function (e) {
            ViewOutRequestVariable.btnEditDetail();
        });

        $('#btnDeleteDetalle').click(function (e) {
            clearErrors();
            var index = $(this).attr("data-idDetalleSolicitud");
            ViewOutRequestVariable.removeDetailOutRequest(index);
            return false;
        });

        $("#assetNameText").keydown(function (e) {
            if (e.keyCode == 13) {
                ViewOutRequestVariable.guardarArticulosCodeBar();
            }
        });


        $("#codeBarText").keydown(function (e) {
            if (e.keyCode == 13) {
                ViewDeliverOutRequestVariable.addCodeBarAsset($("#codeBarText").val());
            }
        });


        //---- Errors Alerts
        function clearErrors() { $('#msgErrorAnyModal').html(''); $('#IndexAlerts').html(''); $('#detalleInventarioAlerts').html(''); }

        // Main Search, typing timer debouce
        this.searchDebounceProject = function (e) {
            $.ajax({
                url: searchDebounceProjectUrl,
                type: "POST",
                cache: false,
                data: { query: $("#searchBoxProject").val() },
                success: function (data) {
                    $("#anyListUpdate").html(data);
                }, error: function (err) { writeError("IndexAlerts", "¡Error al consultar los Proyectos!", "error"); }
            });
            return false;
        };

        $('#saveOutRequestDetail').click(function () {
            //if ($(this).find('.input-validation-error').length == 0) {
            //    $(this).find(':submit').attr('disabled', 'disabled');
            //}
            abp.ui.setBusy();
            //ViewOutRequestVariable.setOutRequest($("#outRequestIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
            //    $("#notasAdicionalesTxt").val(), $("#typeOutRequestHidden").val(), $("#stateRequestHidden").val(), $("#cellarIdHidden").val(), $("#projectIdHidden").val(),
            //    $("#dueDateText").val(), $("#personInChargeText").val(), $("#contractorIdHidden").val());
            $.post(editDetailPostUrl, { entity: JSON.stringify(ViewOutRequestVariable.getOutRequest()) }, function (data, status) {
                if (data.result.errorCode == 1) {
                    $("#confirmationModal").modal("show");
                    abp.ui.clearBusy();
                }
                if (data.result.errorCode == -2) {
                    $("#SaveRequestData").removeAttr('disabled', 'disabled');
                    writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
                    abp.ui.clearBusy();
                }
                if (data.result.errorCode == -1) {
                    $("#SaveRequestData").removeAttr('disabled', 'disabled');
                    writeErrorAlert('detalleInventarioAlerts', data.result.errorDescription, 'error');
                    abp.ui.clearBusy();
                }
            }).error(function (error, status) {
                $(this).find(':submit').removeAttr('disabled');
                $(this).find(':submit').removeAttr('disabled', 'disabled');
                writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
                abp.ui.clearBusy();
            });
            return false;
        });


        $('#SaveRequestData').click(function (e) {
            console.log('SaveRequestData');
            //if ($(this).find('.input-validation-error').length == 0) {
            //    $(this).find(':submit').attr('disabled', 'disabled');
            //}
            abp.ui.setBusy();
            ViewOutRequestVariable.setOutRequest($("#outRequestIdHidden").val(), $("#noRequestHidden").val(), $("#noRequestHidden").val(), $("#notasAdicionalesTxt").val(),
                $("#commentsTxt").val(), $("#typeOutRequestHidden").val(), $("#stateRequestHidden").val(), $("#cellarIdHidden").val(), $("#projectIdHidden").val(),
                $("#dueDateText").val(), $("#responsiblePersonHidden").val(), $("#delivertToText").val(), $("#contractorIdHidden").val());
            
           var dataImage = new FormData();
            var files1 = $("#uploadEditorImage1").get(0).files;
            if (files1.length > 0) {
                dataImage.append("OutRequestImagesSection1", files1[0]);
            }
            var files2 = $("#uploadEditorImage2").get(0).files;
            if (files2.length > 0) {
                dataImage.append("OutRequestImagesSection2", files2[0]);
            }
           /* var files3 = $("#uploadEditorImage3").get(0).files;
            if (files3.length > 0) {
                dataImage.append("OutRequestImagesSection3", files3[0]);
            }
            var files4 = $("#uploadEditorImage4").get(0).files;
            if (files4.length > 0) {
                dataImage.append("OutRequestImagesSection4", files4[0]);
            }
            dataImage.append("Comment", $("input[name='Comment']").val());*/
            //dataImage.append("Comment", $("#commentsTxt").val());
            dataImage.append("SignatureStr", ViewOutRequestVariable.getSignatureData());
            dataImage.append("OutRequestDataSection", JSON.stringify(ViewOutRequestVariable.getOutRequest()));

          //  console.log(dataImage);

            $.ajax({
                url: $(this).attr('action'),
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

            //$.post(createRequest, { entity: JSON.stringify(ViewOutRequestVariable.getOutRequest()) }, function (data, status) {
            //    if (data.result.errorCode == 1) {
            //        $("#confirmationModal").modal("show");
            //        abp.ui.clearBusy();
            //    }
            //     if (data.result.errorCode == -2) {
            //        $("#SaveRequestData").removeAttr('disabled', 'disabled');
            //        writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
            //        abp.ui.clearBusy();
            //    }
            //    if (data.result.errorCode == -1) {
            //        $("#SaveRequestData").removeAttr('disabled', 'disabled');
            //        writeErrorAlert('detalleInventarioAlerts', data.result.errorDescription, 'error');
            //        abp.ui.clearBusy();
            //    }
            //}).error(function (error, status) {
            //    $(this).find(':submit').removeAttr('disabled');
            //    $(this).find(':submit').removeAttr('disabled', 'disabled');
            //    writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
            //    abp.ui.clearBusy();
            //});
            return false;
        });

        $('#SaveRequestDataEdit').click(function (e) {
            console.log('SaveRequestDataEdit');
            //if ($(this).find('.input-validation-error').length == 0) {
            //    $(this).find(':submit').attr('disabled', 'disabled');
            //}
            abp.ui.setBusy();
            ViewOutRequestVariable.setOutRequest($("#outRequestIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
                $("#notasAdicionalesTxt").val(), $("#commentsTxt").val(), $("#typeOutRequestHidden").val(), $("#stateRequestHidden").val(), $("#cellarIdHidden").val(), $("#projectIdHidden").val(),
                $("#dueDateText").val(), $("#responsiblePersonHidden").val(), $("#delivertToText").val(), $("#contractorIdHidden").val());

            var dataImage = new FormData();
            var files1 = $("#uploadEditorImage1").get(0).files;
            if (files1.length > 0) {
                dataImage.append("OutRequestImagesSection1", files1[0]);
            }
            var files2 = $("#uploadEditorImage2").get(0).files;
            if (files2.length > 0) {
                dataImage.append("OutRequestImagesSection2", files2[0]);
            }
           /* var files3 = $("#uploadEditorImage3").get(0).files;
            if (files3.length > 0) {
                dataImage.append("OutRequestImagesSection3", files3[0]);
            }
            var files4 = $("#uploadEditorImage4").get(0).files;
            if (files4.length > 0) {
                dataImage.append("OutRequestImagesSection4", files4[0]);
            }*/
            dataImage.append("Comment", $("input[name='Comment']").val());
            var test = ViewOutRequestVariable.getOutRequest();
            dataImage.append("OutRequestDataSection", JSON.stringify(ViewOutRequestVariable.getOutRequest()));
            dataImage.append("SignatureStr", ViewOutRequestVariable.getSignatureData());
            console.log("Aqui vamos papillo");
            $.ajax({
                
                url: $(this).attr('action'),
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
            /*$.post(editRequest, { entity: JSON.stringify(ViewOutRequestVariable.getOutRequest()) }, function (data, status) {
                if (data.result.errorCode == 1) {
                    $("#confirmationModal").modal("show");
                    abp.ui.clearBusy();
                }
                if (data.result.errorCode == -2) {
                    $("#SaveRequestData").removeAttr('disabled', 'disabled');
                    writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
                    abp.ui.clearBusy();
                }
                if (data.result.errorCode == -1) {
                    $("#SaveRequestData").removeAttr('disabled', 'disabled');
                    writeErrorAlert('detalleInventarioAlerts', data.result.errorDescription, 'error');
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

        $('#deliverOutRequestDetail').click(function () {
            console.log('deliverOutRequestDetail');
            abp.ui.setBusy();
            ViewDeliverOutRequestVariable.setOutRequest($("#outRequestIdHidden").val());

            var dataImage = new FormData();
            var files1 = $("#uploadEditorImage1").get(0).files;
            if (files1.length > 0) {
                dataImage.append("OutRequestImagesSection1", files1[0]);
            }
            var files2 = $("#uploadEditorImage2").get(0).files;
            if (files2.length > 0) {
                dataImage.append("OutRequestImagesSection2", files2[0]);
            }
            var files3 = $("#uploadEditorImage3").get(0).files;
            if (files3.length > 0) {
                dataImage.append("OutRequestImagesSection3", files3[0]);
            }
            var files4 = $("#uploadEditorImage4").get(0).files;
            if (files4.length > 0) {
                dataImage.append("OutRequestImagesSection4", files4[0]);
            }
            console.log($("input[name='Comment']").val());
            console.log($("#commentsTxt").val());

            dataImage.append("Comment", $("#commentsTxt").val());
            dataImage.append("OutRequestDataSection", JSON.stringify(ViewDeliverOutRequestVariable.getOutRequest()));
            dataImage.append("SignatureStr", ViewDeliverOutRequestVariable.getSignatureData());

            $.ajax({
                url: deliverRequestUrl,
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
        });

    

        $('#imageditable').click(function () {
            console.log('imageditable');
            abp.ui.setBusy();
            ViewDeliverOutRequestVariable.setOutRequest($("#outRequestIdHidden").val());

            var dataImage = new FormData();
            var files1 = $("#uploadEditorImage1").get(0).files;
            if (files1.length > 0) {
                dataImage.append("OutRequestImagesSection1", files1[0]);
            }
            var files2 = $("#uploadEditorImage2").get(0).files;
            if (files2.length > 0) {
                dataImage.append("OutRequestImagesSection2", files2[0]);
            }
            var files3 = $("#uploadEditorImage3").get(0).files;
            if (files3.length > 0) {
                dataImage.append("OutRequestImagesSection3", files3[0]);
            }
            var files4 = $("#uploadEditorImage4").get(0).files;
            if (files4.length > 0) {
                dataImage.append("OutRequestImagesSection4", files4[0]);
            }
           // console.log($("input[name='Comment']").val());
            //console.log($("#commentsTxt").val());

            dataImage.append("Comment", $("#commentsTxt").val());
            dataImage.append("OutRequestDataSection", JSON.stringify(ViewDeliverOutRequestVariable.getOutRequest()));
            dataImage.append("SignatureStr", ViewDeliverOutRequestVariable.getSignatureData());

            $.ajax({
                url: editarimagenes,
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
        });

        //$('#EditSalidaInventarioForm').submit(function () {
        //    if ($(this).find('.input-validation-error').length == 0) {
        //        $(this).find(':submit').attr('disabled', 'disabled');
        //    }
        //    abp.ui.setBusy();
        //    ViewOutRequestVariable.setOutRequest($("#outRequestIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
        //         $("#notasAdicionalesTxt").val(), $("#typeOutRequestHidden").val(), $("#stateRequestHidden").val(), $("#cellarIdHidden").val(),
        //         $("#projectIdHidden").val(), $("#dueDateText").val(), $("#personInChargeText").val(), $("#contractorIdHidden").val());
        //    $.post($(this).attr('action'), { entity: JSON.stringify(ViewOutRequestVariable.getOutRequest()) }, function (data, status) {
        //        if (data.result.errorCode == 1) {
        //            $("#confirmationModal").modal("show");
        //            abp.ui.clearBusy();
        //        }
        //        if (data.result.errorCode == -1) {
        //            $("#SaveRequestData").removeAttr('disabled', 'disabled');
        //            writeError('detalleInventarioAlerts', data.result.errorDescription, 'error');
        //            abp.ui.clearBusy();
        //        }
        //    }).error(function (error, status) {
        //        $("#SaveRequestData").removeAttr('disabled', 'disabled');
        //        writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
        //        $('body').removeClass("loading");
        //    });
        //    return false;
        //});

        $('#btnCloseRequest').click(function () {
            return false;
        });

        $('a.btnEditMainForm').click(function (e) {
            clearErrors();
            $("#anyPanelBody").html("");
            if (!isEmpty(editUrl)) {
                var id = $(this).attr("data-idSalidaInventario");
                var url = editUrl + '/' + id;
                getRequestNoModal(url);
            }
            return false;
        });
        $('a.btnCreateMainForm').click(function (e) {
            clearErrors();
            $("#anyPanelBody").html("");
            if (!isEmpty(createUrl)) {
                getRequestNoModal(createUrl);
            }
            return false;
        });

        $('a.btnDetailMainForm').click(function (e) {
            clearErrors();
            if (!isEmpty(detailUrl)) {
                var id = $(this).attr("data-idSalidaInventario");
                var url = detailUrl + '/' + id;
                getRequestNoModal(url);
            }
            return false;
        });

        $('a.btnCloseFormMainPanel').click(function (e) {
            $("#anyPanelBody").html('');
            return false;
        });

        $("#signatureRequest").jSignature({ 'height': '400', 'width': '100%' });

        $('#btnFirmaDigital').click(function (e) {
            var signature = ViewDeliverOutRequestVariable.getSignatureUI();
            $("#signatureRequest").jSignature("setData", "data:" + signature);
            $('#signatureModal').modal('show');
            $("#signatureRequest").resize();
            return false;
        });

      

        $("#resetSignature").click(function (e) { $("#signatureRequest").jSignature("reset"); });

        $("#acceptSignature").click(function (e) {
            var tempSigData = $("#signatureRequest").jSignature("getData", "image");
            //$("#signatureDataHidden").val(tempSigData);
            ViewDeliverOutRequestVariable.setSignatureData(tempSigData);
            $('#signatureModal').modal('hide');
        });

        this.searchDebounce = function () {
            ViewOutRequestCloseVariable.searchDebounce();
        }

        this.deleteFunc = function (btn) {
            clearErrors();
            $('#DeleteConfirmationMain_modal').modal('show');
            $('#ItemToDeleteMain_hidden').val($(btn).attr("data-idrequest"));
            return false;
        }

        this.editDetail = function (btn) {
            var idDetail = $(btn).attr("data-idrequest");
            clearErrors();
            $.ajax({
                url: editDetailUrl,
                type: "POST",
                cache: false,
                data: { id: idDetail },
                success: function (data) {
                    $("#anyListDetailsEntity").html(data);
                    $("#editOutRequestDetail").hide();
                    $("#saveOutRequestDetail").removeAttr("style");
                }, error: function (err) { writeError("IndexAlerts", "¡Error al consultar las Solicitudes!", "error"); }
            });
            return false;
        }

        this.deliverDetail = function (assetId, impreso) {
            console.log("deliverDetail");
            clearErrors();
            ViewDeliverOutRequestVariable.deliverAssetId(assetId,impreso);
            return false;
        }

        this.closeRequestApply = function (btn) {
            clearErrors();
            $('#btnCloseRequest').attr('disabled', 'disabled');
            abp.ui.setBusy();
            ViewOutRequestCloseVariable.setOutRequest($("#outRequestIdHidden").val(), $("#cellarIdHidden").val());
            $.post(appyCloseRequests, { entity: JSON.stringify(ViewOutRequestCloseVariable.getOutRequest()) }, function (data, status) {
                if (data.result.errorCode == 1) {
                    updateRequestsListLocal();
                    writeError('IndexAlerts', data.result.errorDescription, 'success');
                    $('#anyModalCloseRequest').modal('hide');
                    abp.ui.clearBusy();
                }
                if (data.result.errorCode == -1) {
                    $("#btnCloseRequest").removeAttr('disabled', 'disabled');

                    writeError('closeRequestAlerts', data.result.errorDescription, 'error');
                    abp.ui.clearBusy();
                }
            }).error(function (error, status) {
                $("#btnCloseRequest").removeAttr('disabled', 'disabled');
                writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
                abp.ui.clearBusy();
            });
        }

        this.closeFunc = function (btn) {
            clearErrors();
            $.ajax({
                url: showCloseRequest,
                type: "POST",
                cache: false,
                data: { outRequestId: $(btn).attr("data-idrequest") },
                success: function (data) {
                    $('#anyModalCloseRequest').html(data);
                    $('#anyModalCloseRequest').modal('show');
                }, error: function (err) { writeError('msgErrorAnyModal', err, 'error'); }
            });
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

        this.cancelacionfun = function (btn) {
            clearErrors();
            $('#calcelar_solicitud_modal').modal('show');
            $('#Itemcancelar_hidden').val($(btn).attr("data-idrequest"));
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
                data: { outRequestId: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        updateRequestsListLocal();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'error');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al aplicar la solicitud', 'error'); abp.ui.clearBusy(); }
            });
        });

        $('#btnModalOkApprovedConfirmation').click(function (e) {
            $('#ApprovedConfirmationMain_modal').modal('hide');
            abp.ui.setBusy();
            var id = $('#ItemToApprovedMain_hidden').val();
            $.ajax({
                url: approvedUrl + '/',
                type: "POST",
                cache: false,
                data: { outRequestId: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        updateRequestsListLocal();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'error');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al aplicar la solicitud', 'error'); abp.ui.clearBusy(); }
            });
        });
       //cancelacion 
        $('#btnModalcencelaApprovedConfirmation').click(function (e) {
            $('#calcelar_solicitud_modal').modal('hide');
            abp.ui.setBusy();
            var id = $('#Itemcancelar_hidden').val();
            $.ajax({
                url: approvedUrl + '/',
                type: "POST",
                cache: false,
                data: { outRequestId: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        updateRequestsListLocal();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'error');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al cencelar la solicitud', 'error'); abp.ui.clearBusy(); }
            });
        });

        $('#btnModalOkDisprobalConfirmation').click(function (e) {
            $('#ApprovedConfirmationMain_modal').modal('hide');
            abp.ui.setBusy();
            var id = $('#ItemToApprovedMain_hidden').val();
            $.ajax({
                url: disprovedUrl + '/',
                type: "POST",
                cache: false,
                data: { outRequestId: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        updateRequestsListLocal();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'error');
                    }
                },
                error: function (xhr, ajaxOptions, thrownError) { writeError('IndexAlerts', 'Error al aplicar la solicitud', 'error'); abp.ui.clearBusy(); }
            });
        });

        $('#btnModalOkDeliverConfirmation').click(function (e) {
            $('#DeliverConfirmationMain_modal').modal('hide');
            abp.ui.setBusy();
            var id = $('#ItemToDeliverMain_hidden').val();
            $.ajax({
                url: deliverUrl + '/',
                type: "POST",
                cache: false,
                data: { outRequestId: id },
                success: function (data) {
                    abp.ui.clearBusy();
                    if (data.result.errorCode == 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'success');
                        updateRequestsListLocal();
                    } else if (data.result.errorCode < 0) {
                        writeError('IndexAlerts', data.result.errorDescription, 'error');
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

        function writeErrorAlert(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
            } else { abp.notify.warn(msg, ""); }
        }

        function writeError(control, msg, type) {
            if (type === "success") {
                abp.notify.success(msg, "");
            } else if (type === "error") {
                abp.notify.error(msg, "");
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }
    });
})();
