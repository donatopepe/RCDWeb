var MessageBoxDialogs = {};

MessageBoxDialogs.ErrorIcon = '/Gradimento/Content/Dialog/icon-error.gif';
MessageBoxDialogs.InfoIcon = '/Gradimento/Content/Dialog/icon-info.gif';
MessageBoxDialogs.QuestionIcon = '/Gradimento/Content/Dialog/icon-question.gif';
MessageBoxDialogs.WarningIcon = '/Gradimento/Content/Dialog/icon-warning.gif';
MessageBoxDialogs.LoadingIcon = '/Gradimento/Content/Dialog/ajax-loader.gif';





var AlertDialog = function () {
    // Settings passed into the dialog.
    var _settings = null;

    /// <summary>
    /// Show the AlertDialog.
    /// <summary>
    /// <param name="settings" type="JSON object">
    /// Contains all the settings for the dialog.  Settings are:
    ///     - message [Required] : Message to be displayed to the user.
    ///     - icon [Required] : MessageBoxDialogs.xyz where xyz represents one of the icon types listed above.
    ///     - parent [Optional] : the parent of the dialog as a jQuery object.
    ///     - callback [Optional] : the function that gets called when the dialog is closed.</param>
    ///     - height [Optional] : New height for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 100px.
    ///     - width [Optional] : New width for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 300px.
    // </param>
    var ShowWindow = function (settings) {
        _settings = settings;

        try {
            if (_settings.height != null) {
              $('#AlertDialog').data('kendoWindow').setHeight(_settings.height);
            }
            if (_settings.width != null) {
              $('#AlertDialog').data('kendoWindow').setWidth(_settings.width);
                $('#AlertDialog_MessageColumn').css('max-width', (parseInt(_settings.width.replace('px', '') - 60) + 'px'));
                $('#AlertDialog_ButtonRow').css('width', (parseInt(_settings.width.replace('px', '') - 10) + 'px'));
            }
            //mette sempre in primo piano l'alert
            $("#AlertDialog").css({ 'z-index': '10000000000' });

            $('#AlertDialog_Image').attr("src", _settings.icon);
            $('#AlertDialog_Message').text(_settings.message);

            // Open the dialog.
            var wnd = $('#AlertDialog').data('kendoWindow');
            wnd.center().open();
        }
        catch (e) {
            alert(e);
        }
    };

    var OkButton_OnClick = function () {
      var wnd = $('#AlertDialog').data('kendoWindow');
        wnd.close();

        if (_settings.callback != null) {
            _settings.callback.call(this);
        }
    };

    var Window_OnActivate = function (e) {
        $('#AlertDialog_OkButton').focus();
    };

    return {
        ShowWindow: ShowWindow,
        OkButton_OnClick: OkButton_OnClick,
        Window_OnActivate: Window_OnActivate
    };
} ();





var ConfirmDialog = function () {
    // Settings passed into the dialog.
    var _settings = null;

    /// <summary>
    /// Show the ConfirmDialog.
    /// <summary>
    /// <param name="settings" type="JSON object">
    /// Contains all the settings for the dialog.  Settings are:
    ///     - message [Required] : Message to be displayed to the user.
    ///     - icon [Optional] : MessageBoxDialogs.xyz where xyz represents one of the icon types listed above.
    ///     - parent [Optional] : the parent of the dialog as a jQuery object.
    ///     - callback [Optional] : the function that gets called when the dialog is closed.</param>
    ///     - height [Optional] : New height for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 100px.
    ///     - width [Optional] : New width for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 300px.
    // </param>
    var ShowWindow = function (settings) {
        _settings = settings;

        try {
            if (_settings.height != null) {
              $('#ConfirmDialog').data('kendoWindow').setHeight(_settings.height);
            }
            if (_settings.width != null) {
              $('#ConfirmDialog').data('kendoWindow').setWidth(_settings.width);
                $('#ConfirmDialog_MessageColumn').css('max-width', (parseInt(_settings.width.replace('px', '') - 60) + 'px'));
                $('#ConfirmDialog_ButtonRow').css('width', (parseInt(_settings.width.replace('px', '') - 10) + 'px'));
            }

            $('#ConfirmDialog_Image').attr("src", _settings.icon == null ? MessageBoxDialogs.QuestionIcon : _settings.icon);
            $('#ConfirmDialog_Message').text(_settings.message);

            // Open the dialog.
            var wnd = $('#ConfirmDialog').data('kendoWindow');
            wnd.center().open();
        }
        catch (e) {
            alert(e);
        }
    };

    var Button_OnClick = function (button) {
      var wnd = $('#ConfirmDialog').data('kendoWindow');
        wnd.close();

        if (_settings.callback != null) {
            _settings.callback.call(this, { result: button });
        }
        
    };

    var Window_OnActivate = function (e) {
        $('#ConfirmDialog_YesButton').focus();
    };

    return {
        ShowWindow: ShowWindow,
        Button_OnClick: Button_OnClick,
        Window_OnActivate: Window_OnActivate
    };
} ();





var OkCancelDialog = function () {
    // Settings passed into the dialog.
    var _settings = null;


    /// <summary>
    /// Show the OkCancelDialog.
    /// <summary>
    /// <param name="settings" type="JSON object">
    /// Contains all the settings for the dialog.  Settings are:
    ///     - message [Required] : Message to be displayed to the user.
    ///     - icon [Optional] : MessageBoxDialogs.xyz where xyz represents one of the icon types listed above.
    ///     - parent [Optional] : the parent of the dialog as a jQuery object.
    ///     - callback [Optional] : the function that gets called when the dialog is closed.</param>
    ///     - height [Optional] : New height for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 100px.
    ///     - width [Optional] : New width for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 300px.
    // </param>
    var ShowWindow = function (settings) {
        _settings = settings;

        try {
            if (_settings.height != null) {
              $('#OkCancelDialog').data('kendoWindow').setHeight(_settings.height);
            }
            if (_settings.width != null) {
              $('#OkCancelDialog').data('kendoWindow').setWidth(_settings.width);
                $('#OkCancelDialog_MessageColumn').css('max-width', (parseInt(_settings.width.replace('px', '') - 60) + 'px'));
                $('#OkCancelDialog_ButtonRow').css('width', (parseInt(_settings.width.replace('px', '') - 10) + 'px'));
            }

            $('#OkCancelDialog_Image').attr("src", _settings.icon == null ? MessageBoxDialogs.QuestionIcon : _settings.icon);
            $('#OkCancelDialog_Message').text(_settings.message);

            // Open the dialog.
            var wnd = $('#OkCancelDialog').data('kendoWindow');
            wnd.center().open();
        }
        catch (e) {
            alert(e);
        }
    };

    /// <summary>
    /// Event handler for when the OK or Cancel button is clicked.
    /// <summary>
    /// <param name="button" type="string">"OK" if the OK button was clicked, "Cancel" if the Cancel button was clicked.</param>
    var Button_OnClick = function (button) {
      var wnd = $('#OkCancelDialog').data('kendoWindow');
        wnd.close();

        //        if (_settings.callback != null) {
        //            _settings.callback.call(this, { result: button });
        //        }
        return button;

    };

    var Window_OnActivate = function (e) {
        $('#OkCancelDialog_OkButton').focus();
    };

    return {
        ShowWindow: ShowWindow,
        Button_OnClick: Button_OnClick,
        Window_OnActivate: Window_OnActivate
    };
} ();






var WaitDialog = {};

// Settings passed into the dialog.
WaitDialog._settings = null;

/// <summary>
/// Show the WaitDialog.
/// <summary>
/// <param name="settings" type="JSON object">
/// Contains all the settings for the dialog.  Settings are:
///     - message [Required] : Message to be displayed to the user.
///     - parent [Optional] : the parent of the dialog as a jQuery object.
///     - callback [Optional] : the function that gets called when the dialog is closed.</param>
///     - height [Optional] : New height for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 100px.
///     - width [Optional] : New width for the Confirm dialog.  Format should be <value>px.  For example: 500px.  Default is 300px.
// </param>
WaitDialog.ShowWindow = function(settings) {
    WaitDialog._settings = settings;

    try {
        if (WaitDialog._settings.height != null) {
          $('#WaitDialog').data('kendoWindow').setHeight(WaitDialog._settings.height);
        } else {
            $('#WaitDialog').find('div.t-window-content').height('100px');
        }

        if (WaitDialog._settings.width != null) {
          $('#WaitDialog').data('kendoWindow').setWidth(WaitDialog._settings.width);
            $('#WaitDialog_MessageColumn').css('max-width', (parseInt(WaitDialog._settings.width.replace('px', '') - 60) + 'px'));
        } else {
            $('#WaitDialog').find('div.t-window-content').width('300px');
            $('#WaitDialog_MessageColumn').css('max-width', '240px');
        }

        $('#WaitDialog_Image').attr("src", MessageBoxDialogs.LoadingIcon);
        $('#WaitDialog_Message').text(WaitDialog._settings.message);

        var wnd = $('#WaitDialog').data('kendoWindow');
        wnd.center().open();
    } catch(e) {
        alert(e);
    }
};

WaitDialog.CloseWindow = function(callback) {
  var wnd = $('#WaitDialog').data('kendoWindow');
    wnd.close();

    if (callback != null) {
        WaitDialog._settings.callback = callback;
    }

    if (WaitDialog._settings.callback != null) {
        WaitDialog._settings.callback.call(this);
    }
};


function confirmDialog(msg, divIdDialog, divIdSubmit) {
    setConfirmDialogText(msg, divIdDialog);

    $("#" + divIdDialog).dialog({
        resizable: false,
        height: 250,
        width: 370,
        modal: true,
        buttons: {
            Ok: function () {
                $(this).dialog("close");
                if (divIdSubmit != "" && divIdSubmit != null) {
                    $("#" + divIdSubmit).submit();
                }
            },
            Annulla: function () {
                $(this).dialog("close");
            }
        }
    });
};


function alertDialog(msg, divIdDialog, focusId) {
    setConfirmDialogText(msg, divIdDialog);

    $("#" + divIdDialog).dialog({
        resizable: false,
        height: 200,
        width: 364,
        modal: true,
        buttons: {
            Ok: function () {
              $(this).dialog("close");
              $('#loadImage_div').attr('style', 'display:none');
                if (focusId != "") {
                    focusRed(focusId);
                }

            }
        },
        zIndex:1000000
    });
};

    function focusRed(focusId){
            $("#" + focusId).focus();
            $("#" + focusId).css({ 'background-color': 'red' });
            $("#" + focusId).change(function() {
                $("#" + focusId).css({ 'background-color': 'white' });
           });
    };

function setConfirmDialogText(msg, divIdDialog) {
    
    $("#" + divIdDialog + " p:first-child").html(' ');
    var msgTot = "<span><img src=" + MessageBoxDialogs.QuestionIcon + ">	</span>";
    //var msgTot = '<span class="ui-icon ui-icon-alert" style="float:left; margin:0 7px 20px 0;"></span>';
    msgTot += msg;

    $("#" + divIdDialog + " p:first-child").append(msgTot);
};

function CheckOrdine(numordine, divId) {
    var blnResult = true;

    if ((numordine.length > 18) || (numordine.length < 18)) {
        
        //$("#" + divId).val('');
//        $("#" + divId).focus();
//        $("#" + divId).css({ 'background-color': 'red' });
//        $("#" + divId).keypress(function () {
//            $("#" + divId).css({ 'background-color': 'white' });
        //        });
        blnResult = false;
        return blnResult;
    }
    blnResult = IsNumeric(numordine, divId);
    return blnResult;
};


function CheckCodRuolo(ruolo, divId) {
    var blnResult = true;

    if ((ruolo.length > 3) || (ruolo.length < 3)) {
        
        blnResult = false;
        return blnResult;
    }
    
    return blnResult;
};


function CheckEMail(address, divId) {

    var blnResult = true;

    var reg = /^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    if (reg.test(address) == false) {
        //$("#" + divId).val('');
        blnResult = false;
         
    }
    return blnResult;
};

function IsNumeric(strString, divId) {
    var strValidChars = "0123456789";
    var strChar;
    var blnResult = true;

    if (strString.length == 0) return false;

    //  test strString consists of valid characters listed above
    for (i = 0; i < strString.length && blnResult == true; i++) {
        strChar = strString.charAt(i);
        if (strValidChars.indexOf(strChar) == -1) {
            blnResult = false;
            //$("#" + divId).val('');
        }
    }

    if (divId.toLowerCase().indexOf("provincia")>=0) {
        if ((strString.length > 2) || (strString.length < 2)) {
            blnResult = true;
            
        }
    }
    return blnResult;
};


function IsLimiteAzioni(azioni) {
    var blnResult = true;
    var azInt = parseFloat(azioni);
    if(azInt<100) {
        blnResult = false;
    }
    return blnResult;
};

function IsCodFiscale(codice, divId) {
    
    var blnResult = true;

    var reg = /^[a-zA-Z]{6}\d{2}[a-zA-Z]{1}\d{2}[a-zA-Z]{1}\d{3}[a-zA-Z]{1}$/;
    if (reg.test(codice) == false) {
        //$("#" + divId).val('');
        blnResult = false;

    }
    return blnResult;
};


function CheckChar(strString, divId) {
    var strValidChars = "0123456789/";
    var strChar;
    var blnResult = true;

    if (strString.length == 0) return false;

    //  test strString consists of valid characters listed above
    for (i = 0; i < strString.length && blnResult == true; i++) {
        strChar = strString.charAt(i);
        if (strValidChars.indexOf(strChar) == -1) {
            blnResult = false;
            //$("#" + divId).val('');
        }
    }
    return blnResult;
};

function IsValid(strString, divId) {
    //  check for valid numeric strings	
    var strChar;
    var blnResult = true;

    if (strString.length == 0) return false;

    //  test strString consists of valid characters listed above
    for (i = 0; i < strString.length && blnResult == true; i++) {
        strChar = strString.charAt(i);
        if (strString.length == 1 && (strChar == "." || strChar == ",")) {
            blnResult = false;
            //$("#" + divId).val('');
        }
    }
    return blnResult;
};

function isDate(txtDate) {
    var currVal = txtDate;
    if (currVal == '')
        return false;

    //Declare Regex  
    var rxDatePattern = /^(\d{1,2})(\/|-)(\d{1,2})(\/|-)(\d{4})$/;
    var dtArray = currVal.match(rxDatePattern); // is format OK?

    if (dtArray == null)
        return false;

    //Checks for dd/mm/yyyy format.
    dtDay = dtArray[1];
    dtMonth = dtArray[3];
    dtYear = dtArray[5];
    
    //Check age
    var now = new Date();
    var nowYear = now.getFullYear();
    var age = nowYear - dtYear;

    if (dtMonth < 1 || dtMonth > 12)
        return false;
    else if (dtDay < 1 || dtDay > 31)
        return false;
    else if ((dtMonth == 4 || dtMonth == 6 || dtMonth == 9 || dtMonth == 11) && dtDay == 31)
        return false;
    else if (dtMonth == 2) {
        var isleap = (dtYear % 4 == 0 && (dtYear % 100 != 0 || dtYear % 400 == 0));
        if (dtDay > 29 || (dtDay == 29 && !isleap))
            return false;
    } else if (age>110) {
        return false;
    }
    return true;
}

;