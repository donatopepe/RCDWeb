

//Session Warning
  function SessionWarning(sessionTimeout, sessionTimeoutWarning) {
  //minutes left for expiry
  var minutesForExpiry =  (parseInt(sessionTimeout) - parseInt(sessionTimeoutWarning));
  var message = "La sessione scadrà tra " + minutesForExpiry + " minuti.";

  //Confirm se implementato il metodo per estendere la sessione
  answer = alert(message);//confirm(message);

  //if yes, extend the session.
  //if(answer)
  //{
  //  var img = new Image(1, 1);
  //  img.src = 'KeepAlive.aspx?date=' + escape(new Date());

    //  //Clear the RedirectToHomePage method
  //  if (redirectToWelcomePageTimer != null) {
  //    clearTimeout(redirectToWelcomePageTimer);
  //  }
  //  //reset the time on page load
  //  timeOnPageLoad =  new Date();
  //  sessionWarningTimer = setTimeout('SessionWarning()', parseInt(sessionTimeoutWarning) * 60 * 1000);
  //  //To redirect to the welcome page
    //  redirectToWelcomePageTimer = setTimeout('RedirectToHomePage()',parseInt(sessionTimeout) * 60 * 1000);
  //}

  //*************************
  //Even after clicking ok(extending session) or cancel button, 
  //if the session time is over. Then exit the session.
  var currentTime = new Date();
  //time for expiry
  var timeForExpiry = timeOnPageLoad.setMinutes(timeOnPageLoad.getMinutes() + parseInt(sessionTimeout)); 

  //Current time is greater than the expiry time
  if(Date.parse(currentTime) > timeForExpiry)
  {
    alert("Sessione scaduta. Si verrà reindirizzati alla Home Page");
    var appath = GetAppPath();
    var url = "";
    if (appath != "/") {
      url = appath + "/Account/LogOnWithUtenzaTest";
    } else {
      url = "/Account/LogOnWithUtenzaTest";
    }
    window.location = url;
  }
  //**************************
}

//Session timeout
function RedirectToHomePage(){
  alert("Sessione scaduta. Si verrà reindirizzati alla Home Page");
  var appath = GetAppPath();
  var url = "";
  if (appath != "/") {
    url = appath + "/Account/LogOnWithUtenzaTest";
  } else {
    url = "/Account/LogOnWithUtenzaTest";
  }
  window.location = url;
}
