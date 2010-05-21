function AjaxUpdateCallWithFormSubmit(url,updateId,formId){
    var dt = $('#'+formId).serializeArray();
    jQuery.ajax( {
     type: "POST",
     url: url,
     data: dt,
     success: function(msg){
            jQuery("#"+updateId).html(msg);
        }
    });
}

function AjaxCall(url,updateId,blockId,params,requestType,postEval){
if(url==null || updateId==null)
{
    alert('Invalid Ajax call!');
    return false;
}
$.ajax({
      url: url,
      type:requestType,
      cache: false,
      data: params,
      dataType: 'html',
      beforeSend: function(){
         if(blockId!=null)
             $(blockId).block({  
                message: '<h1>Processing</h1>',  
                css: { border: '3px solid #a00' }  
            }); 
       },
      success: function(html){
        if(blockId!=null) $(blockId).unblock();
        $(updateId).html(html);
        if(postEval!=null)eval(postEval);
      },
      error: function (XMLHttpRequest, textStatus, errorThrown) {
          if(blockId!=null) $(blockId).unblock();
          alert(errorThrown);
          }
  });

}
function AjaxUpdateCall(url,updateId,blockId,params){
    AjaxCall(url,updateId,blockId,params,'POST',null)
}

function showIframeDialog(id,title,width,height,buttons){
$("iframe").remove();
var dialog = $('<iframe src="'+id+'" id="printFrame" style="width:700px"/>');
var uiDialog = dialog.dialog({
    title:title,
    autoOpen: true, 
    height: height,
    width: width,
    position:'center',
    buttons: buttons,
    modal:true,
    position:'center',
    overlay: { opacity: 0.5, background: "black"},
});
}

function showRMContentDialog(id,title,width,height,buttons){
   
   $.get(id,function(data){
      $("<div id=\"printDialog\">").append(data).dialog({
            title:title,
            autoOpen: true, 
            height: height,
            width: width,
            position:'center',
            buttons: buttons,
            modal:true,
            position:'center',
            overlay: { opacity: 0.5, background: "black"},
        });
   });

   
}

function adHockFieldAdd(action,prefix,object,updateId){
      $("#adHockAdd").remove();
      $("<div id=\"adHockAdd\">").append("<label>"+object+"</label><input type=\"text\" name=\""+prefix+"\">").dialog({
            title:'Add New '+object,
            autoOpen: true, 
            position:'center',
            buttons: { "Save" : function() {
            
                var dt = $('[name='+prefix+']').serializeArray();
                jQuery.ajax( {
                 type: "GET",
                 url: action,
                 data: dt,
                 success: function(msg){
                        jQuery("#"+updateId).replaceWith(msg);
                    }
                });

                $(this).dialog("close"); 
             }},
            modal:true,
            position:'center',
            overlay: { opacity: 0.5, background: "black"},
        });
}

function showDialog(id,title,width,height){
    createDialog(id,title,width,height);
    $(id).dialog('open'); 
}

function openDialog(id){
    $(id).dialog('open'); 
}



function createDialog(id,title,width,height)
{
$(id).dialog({ 
    title:title,
    autoOpen: false, 
    bgiframe:true,
    height: height,
    width: width,
    resizable:true,
    modal:true,
    position:'center',
    overlay: { opacity: 0.5, background: "black"},
    
    buttons: { 
        "Close": function() { 
            $(this).dialog("close"); 
        } 
    } 
});
}

$().ajaxStop($.unblockUI); 


function initMenu() {
  $('#menu ul').hide();
  $('#menu ul a li ul a li ').hide();
  $('#menu ul:first').show();
  $('#menu li a').click(
      function() {
          var checkElement = $(this).next();
          if((checkElement.is('ul')) && (checkElement.is(':visible'))) {
          return false;
          }
          if((checkElement.is('ul')) && (!checkElement.is(':visible'))) {
          $('#menu ul:visible').slideUp('normal');
          checkElement.slideDown('normal');
          return false;
          }
          }
  );
}				