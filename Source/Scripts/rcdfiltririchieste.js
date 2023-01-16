function controlloDate() {
  var dataOrdineDA = "dataOrdineDA";
  var dataOrdineDAValue = $('#dataOrdineDA').val();

  var dataOrdineAValue = $('#dataOrdineA').val();
  var esito = true;
  if (new Date(dataOrdineDAValue).getTime() > new Date(dataOrdineAValue).getTime()) {
    esito = false;
  }
  if (esito == false) {
    alertDialog('Inserire un intervallo valido', 'confirmDialogFiltri', dataOrdineDA);
  }
  return esito;
}