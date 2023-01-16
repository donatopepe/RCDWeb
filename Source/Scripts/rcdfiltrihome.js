function getFlagUffFiliale(uocodeFiliale) {
  var appath = GetAppPath();
  var url = "";
  if (appath != "/") {
    url = appath + "/Common/Helpers/_GetFlagUo";
  } else {
    url = "/Common/Helpers/_GetFlagUo";
  }
  $.ajax({

    url: url,
    type: 'POST',
    data: { uocode: uocodeFiliale },
    beforeSend: function (jqXHR, settings) {
      //alert("before");
    },
    error: function (jqXHR, textStatus, errorThrown) {
      alert('errore flag: ' + textStatus + ' \n desc: ' + errorThrown);
    },
    success: function (data) {

      $("#FlagUff").val(data);

    }
  });
}

function rebindGestori() {
  $("#ElencoGestoriAutoComplete").val('');
  $("#gestore").val('');

  var mercato = getCheckMercato();

  $("#mercato").val(mercato);
  
  $.ajax({
    url: GetAppPath() + '/RCD/Richiesta/_GestoriBinding',
    type: 'POST',
    data: { istID: $("#SelIstitutoR").val(), flagUffRuolo: $("#FlagUff").val(), myUocode: $("#uocode").val(), mercato: mercato, selFiltriHome: "true" },
    beforeSend: function (jqXHR, settings) {
      //alert("before");
    },
    error: function (jqXHR, textStatus, errorThrown) {
      alert('errore gest: ' + textStatus + ' \n desc: ' + errorThrown);
    },
    traditional: true,
    success: function (data) {

      var gestoriAutocomplete = $("#ElencoGestoriAutoComplete").data("kendoDropDownList");
      if (data != null && data.length != 0) {

        gestoriAutocomplete.setDataSource(data);
        $("#ElencoGestoriAutoComplete").data("kendoDropDownList").enable(true);
      } else {
        gestoriAutocomplete.value('');
        gestoriAutocomplete.setDataSource(null);
      }
	  enableGestori();
    }
  });
}

function rebindGrafici() {
  var mercato = getCheckMercato();




  var GridTipo = $('#GridTipoRichieste').data('kendoGrid');
  GridTipo.dataSource.read();

  var pieStatoRichieste = $('#pieStatoRichieste').data('kendoChart');
  pieStatoRichieste.dataSource.read();

  var scatterStatoRichieste = $('#ScatterLineRichieste').data('kendoChart');
  scatterStatoRichieste.dataSource.read();
}

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

function setIstitutoR() {
  $("#istitutoR").val($("#SelIstitutoR").val());
  $('#SelAreaR').data('kendoDropDownList').select(0);
  //$('#FilialiHubAutoCompleteR').data('kendoDropDownList').select(0);
  $('#FilialiSpokeAutoCompleteR').data('kendoDropDownList').select(0);
  $("#areaR").val('');
  $("#uocode").val('');
  $("#FlagUff").val('');


  //if ($("#SelIstitutoR").val() == "46") {

  //  $("#SelAreaR").data('kendoDropDownList').enable(false);
  //  $("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(true);
  //  $("#nomeHub").text("Filiale Hub/Capofila:");
  //  $("#FilialiHubAutoCompleteR").data("kendoDropDownList").options.optionLabel = "--seleziona  Hub/Capofila--";
  //  //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").refresh();

  //} else
    if ($("#SelIstitutoR").val() != "") {
      $("#SelAreaR").data('kendoDropDownList').enable(true);
      rebindArea();
											
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
					   
		 
																		   
	  
   
    //$("#nomeHub").text("Distretto:");
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").options.optionLabel = "--seleziona  Distretto--";
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").refresh();
    
    //$("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);

  } else {
    $("#SelAreaR").data('kendoDropDownList').enable(false);
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
    //$("#nomeHub").text("Distretto:");
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").options.optionLabel = "--seleziona  Distretto--";
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").refresh();
    
    //$("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);

  }
  $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);
  $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").select(0);
  //rebindGestori();

}

function onloadIstituto() {

  var admin = $("#SelIstitutoR").val();
  if (admin != "0") {
    $("#istitutoR").val($("#SelIstitutoR").val());
    $("#SelIstitutoR").data('kendoComboBox').enable(false);
    
  }
}

function setAreaR() {
  if ($('#SelAreaR').data('kendoDropDownList').value() != "") {
    $("#areaR").val($("#SelAreaR").val());
    $("#FlagUff").val("A");
    $("#uocode").val($("#SelAreaR").val());

    //MOD PRIMA NUOVO MDB
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(true);
    //$("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);
	//rebindFilialiHub($("#filialeHubR").val());

    
    //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
    $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(true);
    rebindFilialiSpoke($("#SelAreaR").val());
  } else {
    //if ($("#SelIstitutoR").val() != "46") {
     // $("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
      //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").select(0);
      $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);
      $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").select(0);
    //}
    //$("#areaR").val('');
    //$("#uocode").val('');
    //$("#FlagUff").val('');
  }


}

function setFilialeHubR() {

  if ($("#FilialiHubAutoCompleteR").data("kendoDropDownList").value() != "") {

    $("#filialeHubR").val($('#FilialiHubAutoCompleteR').val());
    $("#uocode").val($('#FilialiHubAutoCompleteR').val());
    getFlagUffFiliale($('#FilialiHubAutoCompleteR').val());
    $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(true);
    $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").select(0);
    rebindFilialiSpoke($("#filialeHubR").val());
   // if ($("#FilialiSpokeAutoCompleteR-input").val() == ""
		  //|| $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").value() == "") {
    //} else {
      //se non valorizzo l' hub recupero i dati AREA solo se ist 47
      //if ($("#SelIstitutoR").val() == "47") {
      //  $("#filialeHubR").val("");
      //  $("#uocode").val($("#SelAreaR").val());
      //  $("#FlagUff").val("A");
      //} else {

      //  $("#filialeHubR").val("");
      //  $("#uocode").val('');
      //  $("#FlagUff").val('');
      //}

      //if ($("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").value() != "") {

        
      //  $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);

      //}


    //}
    if ($("#SelIstitutoR").val() == "0") {
      $("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
    }
    //if ($("#FilialiHubAutoCompleteR").val() == "") {
    //  $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").select(0);
    //}
  } else {
    $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);
    $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").select(0);
    //if ($("#SelIstitutoR").val() == "47") {
      $("#filialeHubR").val("");
      $("#uocode").val($("#SelAreaR").val());
      $("#FlagUff").val("A");
    //} else {
    //  $("#filialeHubR").val("");
    //  $("#uocode").val('');
    //  $("#FlagUff").val('');
    //}
  }



}

function setFilialeSpokeR() {
  if ($("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").value() != "") {

    $("#filialeSpokeR").val($("#FilialiSpokeAutoCompleteR").val());
    $("#uocode").val($('#FilialiSpokeAutoCompleteR').val());
    $("#FlagUff").val("J");
															 
  } else {

    //se non valorizzo la spoke recupero i dati HUB
    $("#filialeSpokeR").val("");
    if ($('#FilialiHubAutoCompleteR').val() != "") {
      $("#uocode").val($('#FilialiHubAutoCompleteR').val());
      getFlagUffFiliale($('#FilialiHubAutoCompleteR').val());
    }
  }

}

function onloadArea() {
  $("#areaR").val($("#SelAreaR").val());
  
  if ($("#SelIstitutoR").val() == "0") {
    $("#SelAreaR").data('kendoDropDownList').enable(false);
  }
  else {
    //if ($('#FilialiHubAutoCompleteR').val() != "") {
    //  $("#SelAreaR").data('kendoDropDownList').enable(false);
    //}
    if ($('#FilialiSpokeAutoCompleteR').val() != "") {
      $("#SelAreaR").data('kendoDropDownList').enable(false);
    }
    else {
      //filtro area esempio area1 corporate o retail
      //if ($("#filtriArea").val() == "0" && $("#SelIstitutoR").val() != "46") {
      $("#SelAreaR").data('kendoDropDownList').enable(true);
      var are = $("#SelAreaR").data('kendoDropDownList');
     
      if (are._data().length != 0) {
        if (are._data().length == 1) {
          are.select(1);
          are.enable(false);
          $("#areaR").val($("#SelAreaR").val());
            if ($("#SelIstitutoR").val()== "46"){ 
		  //recursiveRebindHub();
          recursiveRebindSpoke();
            }
          
          }else {
          var admin = $("#SelIstitutoR").val();
          if (admin != "0" && $("#SelIstitutoR").prop('disabled')==true && $('#ruo').val()!='PGR' && $('#ruo').val()!='PGC') {
              $("#SelAreaR").data('kendoDropDownList').enable(false);
            }
          }
        }
      
      //} else {
      //  $("#SelAreaR").data('kendoDropDownList').enable(false);
      //  if ($('#FlagUff').val() == 'A')
      //    setTimeout(
      //          function () {
      //            rebindFilialiHub();
      //          }, 1000);

      //}
    }
  }


}

function recursiveRebindHub(){
	if($("#FilialiHubAutoCompleteR").data("kendoDropDownList")!= undefined){
		rebindFilialiHub();
		$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(true);
	}else{
	setTimeout(
               function () {
                  recursiveRebindHub();
                }, 500);
	}
}

function recursiveRebindSpoke(){
    if($("#FilialiSpokeAutoCompleteR").data("kendoDropDownList")!= undefined){
        rebindFilialiSpoke();
        $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(true);
    }else{
        setTimeout(
            function () {
                rebindFilialiSpoke();
            }, 500);
    }
}

function onloadHub() {
  
  var hub = $("#FilialiHubAutoCompleteR").data("kendoDropDownList");
  if (hub._data().length != 0) {
    if (hub._data().length == 1) {
      hub.select(1);
      hub.enable(false);
      if ($("#FilialiHubAutoCompleteR").data("kendoDropDownList").value() != "") {
        $("#filialeHubR").val($('#FilialiHubAutoCompleteR').val());
      }
	  $("#SelAreaR").data('kendoDropDownList').enable(false);
    } else {
      //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(true);
    }
		
																		
  }
  //if ($("#SelIstitutoR").val() == "46") {
  //  $("#nomeHub").text("Filiale Hub/Capofila:");
  //  $("#FilialiHubAutoCompleteR").data("kendoDropDownList").options.optionLabel = "--seleziona  Hub/Capofila--";
  //}

}

function onloadSpoke() {
  var spoke = $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList");
  if ($('#FlagUff').val() != 'F' && $('#FlagUff').val() != 'H') {
    if (spoke._data().length != 0) {
      if (spoke._data().length == 1) {
        spoke.select(1);
        spoke.enable(false);
        //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
        if ($("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").value() != "") {

          $("#filialeSpokeR").val($("#FilialiSpokeAutoCompleteR").val());
          $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").enable(false);
        }
      }
    }
  }
}


function setElencoGestori() {
  $("#gestore").val($('#ElencoGestoriAutoComplete').val());
}

function onElencoGestori() {
  //$("#gestore").val($('#ElencoGestoriAutoComplete').val());
  //$("#ElencoGestoriAutoComplete").data("kendoComboBox").enable(false);

  $("#ElencoGestoriAutoComplete").val('');
  $("#gestore").val('');

  bindDataGestori();

}


function rebindArea(ist) {

 

  var appath = GetAppPath();
  var url = "";
  if (appath != "/") {
    url = appath + "/Common/Helpers/_AreaBinding";
  } else {
    url = "/Common/Helpers/_AreaBinding";
  }

  $.ajax({

    url: url,
    type: 'POST',
    data: { istituto: $("#SelIstitutoR").val() },
    beforeSend: function (jqXHR, settings) {
      //alert("before");
    },
    error: function (jqXHR, textStatus, errorThrown) {
      alert('errore area: ' + textStatus + ' \n desc: ' + errorThrown);
    },
    success: function (data) {

      var areaAtocomplete = $("#SelAreaR").data("kendoDropDownList");
      if (data != null && data.length != 0) {

        areaAtocomplete.setDataSource(data);
      } else {
        areaAtocomplete.value('');
        areaAtocomplete.setDataSource(null);
      }

     
    }
  });

}


function rebindFilialiHub(filialeHub) {

  $("#FilialiHubAutoCompleteR").data("kendoDropDownList").value('');
  $("#filialeHubR").val('');
  $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList").value('');
  $("#filialeSpokeR").val('');

  var appath = GetAppPath();
  var url = "";
  if (appath != "/") {
    url = appath + "/Common/Helpers/_FilialiHubBinding";
  } else {
    url = "/Common/Helpers/_FilialiHubBinding";
  }

  $.ajax({

    url: url,
    type: 'POST',
    data: { istituto: $("#SelIstitutoR").val(), uoArea: $("#areaR").val() },
    beforeSend: function (jqXHR, settings) {
      //alert("before");
    },
    error: function (jqXHR, textStatus, errorThrown) {
      alert('errore filiale: ' + textStatus + ' \n desc: ' + errorThrown);
    },
    success: function (data) {

      var filialiAtocomplete = $("#FilialiHubAutoCompleteR").data("kendoDropDownList");
      if (data != null && data.length != 0) {

        filialiAtocomplete.setDataSource(data);
							 
								 
																				   
		 
      } else {
        filialiAtocomplete.value('');
        filialiAtocomplete.setDataSource(null);
      }

      if ($("#SelIstitutoR").val() == "0") {

        $("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
      }
      //if ($('#SelAreaR').data('kendoDropDownList').value() == "" && $("#SelIstitutoR").val() == 47) {
        //$("#FilialiHubAutoCompleteR").data("kendoDropDownList").enable(false);
      //}
    }
  });

}

function rebindFilialiSpoke(filialeSpoke) {
  $("#FilialiSpokeAutoCompleteR").val('');
  $("#filialeSpokeR").val('');

  var appath = GetAppPath();
  var url = "";
  if (appath != "/") {
    url = appath + "/Common/Helpers/_FilialiSpokeBinding";
  } else {
    url = "/Common/Helpers/_FilialiSpokeBinding";
  }

  $.ajax({

    url: url,
    type: 'POST',
    //data: { UoFilialeHub: $("#filialeHubR").val() },
    data: { UoFilialeHub: $("#SelAreaR").val() },
    beforeSend: function (jqXHR, settings) {
      //alert("before");
    },
    error: function (jqXHR, textStatus, errorThrown) {
      alert('errore spoke: ' + textStatus + ' \n desc: ' + errorThrown);
    },
    success: function (data) {

      var filialiAtocomplete = $("#FilialiSpokeAutoCompleteR").data("kendoDropDownList");
      if (data != null && data.length != 0) {

        filialiAtocomplete.setDataSource(data);
      } else {
        filialiAtocomplete.value("");
        filialiAtocomplete.setDataSource(null);

      }
    }
  });
}


function enableGestori() {

  if ($("#mercato").val().indexOf(',') >= 0) {

    $("#ElencoGestoriAutoComplete").data("kendoDropDownList").enable(true);
  } else {

    //if ($("#mercato").val() != 'R') {

      $("#ElencoGestoriAutoComplete").data("kendoDropDownList").enable(false);
    //}
  }
}

function getCheckMercato() {
  var mercato = "";
  var array = [];

  $('input:checkbox[id^="mer_"]:checked').each(function () {
    array.push($(this).attr("name"));
  });
  for (var i = 0; i < array.length; i++) {
    mercato += array[i];
    if (i != array.length - 1) {
      mercato += ",";
    }
  }
  return mercato;
}
