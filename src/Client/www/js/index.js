!function(t){function e(o){if(n[o])return n[o].exports;var i=n[o]={exports:{},id:o,loaded:!1};return t[o].call(i.exports,i,i.exports,e),i.loaded=!0,i.exports}var n={};return e.m=t,e.c=n,e.p="",e(0)}([function(t,e,n){"use strict";function o(){$("#startbutton").click(function(){s.initSound(),a.send("modstatus",{data:{Recording:!0}})}),$("#stopbutton").click(function(){a.send("modstatus",{data:{Recording:!1}})})}function i(t){var e=$("#startbutton"),n=$("#stopbutton");"Initialized"===t.AIRStatus?(e.attr("disabled",!1),n.attr("disabled",!1)):(e.attr("disabled",!0),n.attr("disabled",!0)),t.Recording?(s.initSoundWithDialog("記録中です。OKをクリックしてください。"),n.show(),e.hide()):(n.hide(),e.show())}n(1);n(2);n(6);var s=n(8),a=n(3),c=n(7);$(function(){c.get(i),c.onChange(i),o()})},function(t,e){"use strict";var n={API_ROOT:"/api/",SOCKET_URL:"ws://"+location.hostname+":8002",COLORMAP_URL:"colormap"};t.exports=n},function(t,e,n){"use strict";function o(){$("#colormap-image").attr("src",s.COLORMAP_URL+"?r="+Math.random())}var i=n(3),s=n(1),a={img:null};$(function(){a.img=$("#thermo-image"),i.onMessage("blob",function(t){var e=new Blob([t],{type:"image/png"});a.img.attr("src",URL.createObjectURL(e))}),i.onMessage("ColorMap",function(t){o()}),o()}),t.exports=a},function(t,e,n){"use strict";function o(t){var e=t.data;if(e.constructor===String){var n=e.match(/^([^\t]+)\t([\s\S]+)/);if(n){var o=n[1];if(f="ConnectionRefused"==o?d:0,u[o]){var i=$.parseJSON(n[2]);$.each(u[o],function(t,e){e(i)})}}else r.log("unknown :"+e)}else e.constructor===Blob?(f=0,u.blob&&$.each(u.blob,function(t,n){n(e)})):r.alert(e,"unknown data")}function i(){if(!l)try{l=new WebSocket(c),l.addEventListener("message",o),l.addEventListener("close",function(){l=null,r.log(l,"socket closed!"),++f<d?setTimeout(i,500):o({data:'ErrorClose	"通信切断しました。"'})})}catch(t){r.alert(t,"websocket failed.")}}var s=n(1),a=s.API_ROOT,c=s.SOCKET_URL,r=n(4),u={},l=null,f=0,d=5,p={onMessage:function(t,e){u[t]||(u[t]=[]),u[t].push(e),i()},send:function(t,e){l&&(e=e||{},$.ajax({url:a+t,async:"undefined"==typeof e.async?!0:e.async,dataType:"json",data:$.extend(!0,{r:Math.random()},e.data),success:function(n){r.log(n,"send("+t+"):success"),n.Success?e.success&&(e.wait?$(function(){e.success(n)}):e.success(n)):(r.log(n,t+" failed."),e.fail?e.wait?$(function(){e.fail(n)}):e.fail(n):alert(t+" failed."))},error:function(t,n,o){r.log([t,n,o],"send error."),e.error&&(e.wait?$(function(){e.error(n,o)}):e.error(n,o))}}))}};t.exports=p},function(t,e){"use strict";var n={log:function(t,e){"object"==typeof console?console.debug(e,t):alert({message:e,obj:t})},alert:function(t,e){n.log(t,e),alert([t,e])}};t.exports=n},function(t,e){"use strict";var n={escape:function(t){return t?(""+t).replace(/[&<>"'\n\r 〜]/g,function(t){return{"&":"&amp;","<":"&lt;",">":"&gt;",'"':"&quot;","'":"&#x27;","\n":"<br>","\r":""," ":"&nbsp;","〜":"&#65374;"}[t]}):""}};t.exports=n},function(t,e,n){"use strict";function o(t){r.Main=t;var e=$("#watchbox1-1"),n=$("#watchbox1-2");if(e.show(),"single"===t.Mode.toLowerCase())n.hide(),e.css({left:t.Left,top:t.Top,width:t.Width,height:t.Height});else{var o=(t.Width-t.Gap)/2;e.css({left:t.Left,top:t.Top,width:o,height:t.Height}),n.css({left:t.Left+o+t.Gap,top:t.Top,width:o,height:t.Height}),n.show()}}function i(t){r.Sub=t;var e=$("#watchbox2");e.css({left:t.Left,top:t.Top,width:t.Width,height:t.Height}),e.show()}function s(){c&&$.each(c,function(t,e){e()})}var a=n(3),c=[],r={Main:null,Sub:null,onChange:function(t){"function"==typeof t&&c.push(t)},updated:function(t){o(t.Main),i(t.Sub),s()}};a.onMessage("Watchbox",function(t){$(function(){o(t.Main),i(t.Sub),s()})}),$(function(){a.send("watchbox",{success:function(t){o(t.Main),i(t.Sub),s()}})}),t.exports=r},function(t,e,n){"use strict";function o(){$.each(a,function(t,e){e(s)})}var i=n(3),s=null,a=[],c={get:function(t){s?t&&t(s):i.send("status",{success:function(e){s=e,t&&t(s),o()}})},onChange:function(t){a.push(t)}};i.onMessage("Status",function(t){s=t,o()}),t.exports=c},function(t,e,n){"use strict";function o(t){"NotConnected"===t.AIRStatus?p.show("赤外線カメラが接続されていません。"):"NotInitialized"===t.AIRStatus?p.show("赤外線カメラが初期化されていません。"):"Initializing"===t.AIRStatus?p.show("赤外線カメラの初期化中です。"):null!==a&&"Initializing"!==a||"Initialized"!==t.AIRStatus||p.show("赤外線カメラの初期化完了しました。",1e3),t.Recording||p.stopAlerm(),a=t.AIRStatus}var i,s,a,c=n(5),r=n(3),u=n(7),l=n(4),f=!1,d=!1,p={show:function(t,e){$("#message-pane .alert.alert-info").remove();var n=$('<div class="alert alert-info">'+c.escape(t)+"</div>");$("#message-pane").prepend(n),e&&setTimeout(function(){n.slideUp(function(){n.remove()})},e)},alert:function(t,e,n){if($("#message-pane .alert.alert-danger").remove(),i&&clearTimeout(i),t){var o=$('<div class="alert alert-danger">'+c.escape(t)+"</div>");$("#message-pane").prepend(o),e&&(i=setTimeout(function(){o.slideUp(function(){o.remove()})},e))}if(n&&f){var a=$("#alerm-sound");a.length&&(s||(s=!0,a[0].play(),l.log(a[0],"alerm play"),setTimeout(function(){a[0].pause(),s=!1},2e3)))}},stopAlerm:function(){if(s){var t=$("#alerm-sound");t[0].pause(),s=!1}},initSound:function(){if(!f){var t=$("#alerm-sound")[0];t.play(),t.pause(),f=!0}},initSoundWithDialog:function(t){if(!f&&!d){d=!0;var e=$('<div><div class="m">'+t+'</div><div class="b"></div></div>');e.css({position:"fixed",top:0,left:0,width:"100%",height:"100%",backgroundColor:"rgba(0,0,0,0.75)"}),e.find(".m").css({color:"white",fontSize:"200%",margin:"100px 20px",textAlign:"center"});var n=$('<button type="button">OK</button>');n.css({padding:"10px 30px",fontSize:"200%",color:"black",borderRadius:4,background:"#ccc"}),e.find(".b").css({textAlign:"center"}).append(n),n.click(function(){p.initSound(),setTimeout(function(){e.remove()},100)}),$("body").append(e)}}};r.onMessage("ErrorMessage",function(t){p.alert(t,3e3)}),r.onMessage("InfoMessage",function(t){p.show(t,3e3)}),r.onMessage("TempAlerm",function(t){p.alert(t,5e3,!0)}),r.onMessage("TempWarn",function(t){p.alert(t,5e3,!1)}),r.onMessage("ConnectionRefused",function(t){p.show(t),$("body").addClass("refused"),$("a,button").prop("disabled",!0),$("a").attr("href","#")}),r.onMessage("ErrorClose",function(t){p.alert(t)}),u.onChange(o),$(function(){u.get(o);var t=$("#alerm-sound");if(t.length){t[0]}}),t.exports=p}]);