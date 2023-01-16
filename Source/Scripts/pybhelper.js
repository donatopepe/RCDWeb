function GetAppPath() {
  var appPath = $("#appPath").text();
  if (appPath != "/") {
    return appPath;
  }
  else {
    return "";
  }
}

function viewRapporto(rapp) {

  var rapporto, html = '';

  rapporto = rapp.toString();
  if (rapporto.length == 13) {
    html = rapporto.substring(0, 2) + '-' + rapporto.substring(2, 5) + '-' + rapporto.substring(5, 13);
  }


  return html;
}

function Grid_onError(e) {
  var grid = $(this).data("kendoGrid");
  // accumulate the error messages in the "message" variable
  var message = "Errore:\n";

  //the text status of the error - "timeout", "error" etc.
  var status = e.status;
  var statusText = e.statusText;
  if (e.xhr) {
    status = e.xhr.status;
    statusText = e.xhr.statusText;
  }

  message += statusText;

  // check if there are any HTTP errors
  if (statusText == "error") {

    if (status == "404") {
      message += " url richiesto non trovato" + "\n";
    }
    if (status == "500") {
      message += " errore server" + "\n";
    }
  }

  if (statusText == "timeout") {
    message += "timeout error" + "\n";
  }

  if (statusText == "parsererror") {
    message += "parser error " + status + "\n";
  }

  // check if there are any model state errors
  if (statusText == "modelstateerror" && e.modelState) {

    // iterate over each key value pair
    $.each(e.modelState, function (key, value) {
      if (value.errors) {
        // iterate over all errors and accumulate them in the "message" variable
        $.each(value.errors, function (index, error) {
          message += error + "\n";
        });
      }
    });

  }
  e.preventDefault();
  alert(message);

}




