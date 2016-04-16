<?php // カメラ温度設定   ?>
<form class="form-horizontal" action="javascript:void(0)" id="signaltemp-form">
  <div id="info3" class="signaltemp-info alert alert-info">測定点の温度を入力して「設定完了」してください。</div>
  <div id="info4" class="signaltemp-info alert alert-success">設定完了しました。</div>

  <fieldset id="monitor1" class="col-sm-6 monitor" >
    <div class="form-group">
      <label class="control-label col-xs-4">設定点1</label>
      <div class="col-xs-2">
        <div id="color-monitor1" class="form-control color-monitor"></div>
      </div>
      <div class="col-xs-4">
        <input type="hidden" name="signal1" />
        <div class="input-group">
          <input type="number" step="0.01" name="temp1" class="form-control" required />
          <div class="input-group-addon">℃</div>
        </div>
      </div>
    </div>
  </fieldset>
  <fieldset id="monitor2" class="col-sm-6 monitor">
    <div class="form-group">
      <label class="control-label col-xs-4">設定点2</label>
      <div class="col-xs-2">
        <div id="color-monitor2" class="form-control color-monitor"></div>
      </div>
      <div class="col-xs-4">
        <input type="hidden" name="signal2" />
        <div class="input-group">
          <input type="number" step="0.01" name="temp2" class="form-control" required />
          <div class="input-group-addon">℃</div>
        </div>
      </div>
    </div>
  </fieldset>
<div class="form-group">
  <div class="col-xs-5 col-xs-offset-1">
    <button type="button" class="btn btn-info pull-left" id="color-startbutton">設定開始</button>
    <button type="button" class="btn btn-warning pull-left" id="color-abortbutton">中断</button>
  </div>
  <div class="col-xs-5">
    <button type="submit" disabled class="btn btn-primary pull-right" id="color-submitbutton">設定完了</button>
  </div>
</div>
</form>
<div id="info-pane">
  <div id="info1" class="signaltemp-info alert alert-info">1つ目の測定点をクリックしてください。</div>
  <div id="info2" class="signaltemp-info alert alert-info">2つ目の測定点をクリックしてください。</div>
</div>