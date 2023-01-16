
/* BEGIN invocazione ottimizzata dell'oggetto Window  */

function gerUrlParametri(param) {
  var url = "";

  if (param == "istituto") {
    url = "/?istituto=" + $("#Istituto").val();
  }

  return url;
}

function apriWindow(id, url, param) {

  if (param != "") {
    url = url + gerUrlParametri(param);
  }
  //$("#" + id).length ? alert('esiste') : alert('non esiste');
  var window = $("#" + id).length ? $("#" + id) : createWindow(id, url);
  window.data('kendoWindow').center().open();
  $(".k-window-content").css("height", "auto").css("width", "auto");
}

function createWindow(id, url) {

  var _width = 900;
  var _height = 600;

  var contentWindow = "<div id=" + id + "><div id='loading' class=overlay>" +
            "<div id='vert-hoz'>" +
            "<TABLE CELLPADDING=0 CELLSPACING=0 bgcolor='#f3f3f3' width='" + _width + "px' height='" + _height + "px'>" +
            "<TR>" +
            "<TD  WIDTH='" + _width + "px' HEIGHT='" + _height + "px'  ALIGN='CENTER' VALIGN='MIDDLE'>" +
            "<BR>" +
            "<span>La pagina sta caricando. Attendere prego...</span>" +
            "<BR><BR>" +
            "<span>Il sistema sta processando le informazioni richieste</span>" +
            "<BR><BR>" +
            "<img src='../Content/img/loading/loading135.gif' border='0' /> <BR>" +
            "</TD>" +
            "</TR>" +
            "</TABLE>" +
            "</div> " +
            "</div></div>";

  return $(contentWindow).kendoWindow({   
    content: url,
    modal: true,
    draggable: true,
    resizable: false,
    width: _width + "px",
    height: _height + "px",
    close: function () { destroyWindow(id); }
  });
}

function createWindowTest(id) {

  $("#" + id).dialog({
    resizable: false,
    height: 600,
    width: 900,
    modal: true,
    open: function () {

      var larghezza = 900;
      var altezza = 600;
      $("#" + id).html(

"<div id='loading' class=overlay>" +
  "<div id='vert-hoz'>" +
  "<TABLE CELLPADDING=0 CELLSPACING=0 bgcolor='#f3f3f3' width='" + larghezza + "px' height='" + altezza + "px'>" +
  "<TR>" +
  "<TD  WIDTH='" + larghezza + "px' HEIGHT='" + altezza + "px'  ALIGN='CENTER' VALIGN='MIDDLE'>" +
  "<BR>" +
  "<span>La pagina sta caricando. Attendere prego...</span>" +
  "<BR><BR>" +
  "<span>Il sistema sta processando le informazioni richieste</span>" +
  "<BR><BR>" +
  "<img src='../Content/img/loading/loading135.gif' border='0' /> <BR>" +
  "</TD>" +
  "</TR>" +
  "</TABLE>" +
  "</div> " +
  "</div>"

);

    },
    zIndex: 1000000
  });


}


function destroyWindow(id) {
  //chiamata su evento onClose dell'oggetto Window
  //serve a distruggere l'oggetto Window e a ri-crearlo alla successiva apertura
  $("#" + id).data('kendoWindow').destroy();

}

function closeWindow(id) {
  //non funziona nell'evento onClose
  //serve a chiudere l'oggetto Window (effettua anche la destroy)
  $("#" + id).data('kendoWindow').close();

}

/* END invocazione ottimizzata dell'oggetto Window */
