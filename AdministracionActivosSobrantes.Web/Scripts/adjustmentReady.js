(function () {

    $(function () {

        var paseInventarioUrl = "/Adjustments/ChangeProcessedStatus";
        var deleteUrl = "/Adjustments/Delete";
        var searchUrl = "/Adjustments/AjaxPage";
        $('#Cellars_DD').change(function () {
            //$('#cellarIdHidden').val($('#Bodegas_DD').val());
            ViewAdjustmentVariable.setAdjustmentCellarId($('#Cellars_DD').val());
        });

        $('#TypeAdjustment_DD').change(function () {
            //$('#typeAdjustmentHidden').val($('#TypeAdjustment_DD').val());
            ViewAdjustmentVariable.setAdjustmentTypeRequest($('#TypeAdjustment_DD').val());
        });

        $('#guardarArticuloBtn').click(function (e) {
            ViewAdjustmentVariable.saveAssets();
        });

        $('#btnShowArticulosList').click(function (e) {
            ViewAdjustmentVariable.btnShowAssetList();
        });

        $('#btnDetalleEdit').click(function (e) {
            ViewAdjustmentVariable.btnEditDetail();
        });

        $('#btnDeleteDetail').click(function (e) {
            ViewAdjustmentVariable.btnEditDetail();
        });

        $('#btnDeleteDetalle').click(function (e) {
            clearErrors();
            var index = $(this).attr("data-idDetalleSolicitud");
            ViewAdjustmentVariable.removeDetailAdjustment(index);
            return false;
        });

        //---- Errors Alerts
        function clearErrors() { $('#msgErrorAnyModal').html(''); $('#IndexAlerts').html(''); $('#detalleInventarioAlerts').html(''); }


        $('#CreateSalidaInventarioForm').submit(function () {
            if ($(this).find('.input-validation-error').length == 0) {
                $(this).find(':submit').attr('disabled', 'disabled');
            }
            abp.ui.setBusy();
            ViewAdjustmentVariable.setAdjustment($("#adjustmentIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
                $("#notestTxt").val(), $("#typeAdjustmentHidden").val(), $("#cellarIdHidden").val(), $("#personInChargeText").val());
            $.post($(this).attr('action'), { entity: JSON.stringify(ViewAdjustmentVariable.getAdjustment()) }, function (data, status) {
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
            });
            return false;
        });

        $('#EditSalidaInventarioForm').submit(function () {
            if ($(this).find('.input-validation-error').length == 0) {
                $(this).find(':submit').attr('disabled', 'disabled');
            }
            abp.ui.setBusy();
            ViewAdjustmentVariable.setAdjustment($("#adjustmentIdHidden").val(), $("#noRequestHidden").val(), $("#requestDocumentNumberTxt").val(),
                $("#notestTxt").val(), $("#typeAdjustmentHidden").val(), $("#cellarIdHidden").val(), $("#personInChargeText").val());
            $.post($(this).attr('action'), { entity: JSON.stringify(ViewAdjustmentVariable.getAdjustment()) }, function (data, status) {
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
                $("#SaveRequestData").removeAttr('disabled', 'disabled');
                writeError('detalleInventarioAlerts', "Error al Guardar la Solicitud", 'error');
                $('body').removeClass("loading");
            });
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

        //-----------------------Modals-----
        //---- Borrado Solicitud
        //$('#btnDeleteMainForm').click(function (e) {
        //    clearErrors();
        //    $('#DeleteConfirmationMain_modal').modal('show');
        //    $('#ItemToDeleteMain_hidden').val($(this).attr("data-idrequest"));
        //    return false;
        //});

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
                data: { adjustmentId: id },
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
                var alert = '<div class="alert alert-danger alert-dismissible" role="alert"><button type="button" class="close" data-dismiss="alert" aria-label="Close"><span aria-hidden="true">&times;</span></button><strong>' + msg + '</strong></div>';
                $("#" + control).html(alert);
            } else { abp.notify.warn(msg, ""); }
        }
    });
})();
