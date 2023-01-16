

function CallTracciatoGarante(loginName, funzioneInterna, filialeRapporto, numeroContoRapporto, ndg, sottoFunzioneInterna, piattaforma, sessionid, codiceServizio,parametriApplicativi) {

  $.ajax({
    dataType: 'jsonp',
    data: {
      loginName: loginName,
      funzioneInterna: funzioneInterna,
      filialeRapporto: filialeRapporto,
      numeroContoRapporto: numeroContoRapporto,
      ndg: ndg,
      sottoFunzioneInterna: sottoFunzioneInterna,
      piattaforma: piattaforma,
      sessionid: sessionid,
      codiceServizio: codiceServizio,
      parametriApplicativi: parametriApplicativi
    },
    jsonp: 'jsonp_callback',
    url: 'http://localhost:12698/TracciatoGarante.ashx',
    //url: 'https://api.example.com/TracciatoGarante/TracciatoGarante.ashx',
    success: function (j) {
      if (j.response != "ok") {
        alert(j.response);
      }
    },
    fail: function (f) {
      alert("errore durante la chiamata a tracciato garante");
    }
  });

}
