function validateFormRichiesta() {

    $('#CreaRichiesta_Form').validate({
        debug:true,
        //Regole
        rules: {
            RTelefono: "required",
            REMail: {
                required: true,
                email: true
            },
            prova:"required"
            
        },
        
        messages: {
            RTelefono: "dddd",
            REMail: "dddd",
            prova: "required"
        }
    });
}