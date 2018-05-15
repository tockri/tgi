<?php
require 'code/pagebase.php';
htmlHead('index');
bodyHead('index');
viewer();
?>
<div>
  <button type="button" class="btn btn-primary btn-lg"
          id="startbutton">
    <span class="glyphicon glyphicon-play"></span>
    開始!!
  </button>
  <button type="button" class="btn btn-warning btn-lg"
          id="stopbutton" style="display:none">
    <span class="glyphicon glyphicon-stop"></span>
    停止
  </button>
  <?php /*
  <button type="button" class="btn btn-danger btn-lg" id="alermtest">
    <span class="glyphicon glyphicon-bell">Test Alert</span>
  </button>
  */ ?>
</div>
<audio id="alerm-sound">
    <source src="sound/alerm3.mp3" type="audio/mpeg" />
</audio>
<?php // <button type="button" onclick="Message.initSound();Message.alert('test', 5000, true);">Test Alert</button> ?>
<?php
bodyFoot();
