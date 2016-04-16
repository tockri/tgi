<?php
require 'code/pagebase.php';
htmlHead('config');
bodyHead('config');
?>
<div class="alert alert-warning">
融着機のローラーが画面下端にわずかに見える程度に、カメラ位置を調節してください。
</div>
<div id="config-form" >
  <ul>
    <li id="tab-main">メイン枠(融着)</li>
    <li id="tab-sub">サブ枠(環境)</li>
    <!--li id="tab-other">その他</li-->
  </ul>
  <div id="config-main" class="tab-pane">
      
    <div class="watchbox-controller">
      <div class="row">
        <label class="col-xs-3 control-label">枠タイプ</label>
        <div class="btn-group col-xs-9">
          <button class="btn btn-default col-xs-6" type="button"
                  id="main-single" value="1">シングル</button>
          <button class="btn btn-default col-xs-6" type="button"
                  id="main-double" value="2">ダブル</button>
        </div>
      </div>
      <div class="row">
        <label class="col-xs-3 control-label">加速</label>
        <div class="col-xs-8">
          <button class="btn btn-default col-xs-8" type="button"
                  id="main-fast">
            <span class="glyphicon glyphicon-forward"></span>
          </button>
        </div>
      </div>
      <div class="row">
        <label class="col-xs-3 control-label">位置</label>
        <div class="col-xs-9" style="padding:0">
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="main-left" value="1">
              <span class="glyphicon glyphicon-arrow-left"></span>
            </button></div>
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="main-up" value="1">
              <span class="glyphicon glyphicon-arrow-up"></span>
            </button></div>
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="main-down" value="1">
              <span class="glyphicon glyphicon-arrow-down"></span>
            </button></div>
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="main-right" value="1">
              <span class="glyphicon glyphicon-arrow-right"></span>
            </button></div>
        </div>
      </div>
      <div class="row">
        <label class="col-xs-3 control-label">幅</label>
        <div class="col-xs-9" style="padding:0">
          <div class="col-xs-6">
            <button class="btn btn-default fill" type="button"
                    id="main-narrow" value="1">▶狭く◀</button>
          </div>
          <div class="col-xs-6">
            <button class="btn btn-default fill" type="button"
                    id="main-wide" value="1">◀広く▶</button>
          </div>
        </div>
      </div>
      <div class="row">
        <label class="col-xs-3 control-label">ダブル幅</label>
        <div class="col-xs-9" style="padding:0">
          <div class="col-xs-6"><button class="btn btn-default fill" type="button"
                                        id="main-gapnarrow" value="1">▶狭く◀</button></div>
          <div class="col-xs-6"><button class="btn btn-default fill" type="button"
                                        id="main-gapwide" value="1">◀広く▶</button></div>
        </div>
      </div>
    </div>
  </div>
  <div id="config-sub" class="tab-pane">
    <div class="watchbox-controller">
      <div class="row">
        <label class="col-xs-3 control-label">加速</label>
        <div class="col-xs-8">
          <button class="btn btn-default col-xs-8" type="button"
                  id="sub-fast">
            <span class="glyphicon glyphicon-forward"></span>
          </button>
        </div>
      </div>
      <div class="row">
        <label class="col-xs-3 control-label">位置</label>
        <div class="col-xs-9" style="padding:0">
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="sub-left" value="1">
              <span class="glyphicon glyphicon-arrow-left"></span>
            </button></div>
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="sub-up" value="1">
              <span class="glyphicon glyphicon-arrow-up"></span>
            </button></div>
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="sub-down" value="1">
              <span class="glyphicon glyphicon-arrow-down"></span>
            </button></div>
          <div class="col-xs-3"><button class="btn btn-default fill" type="button"
                                        id="sub-right" value="1">
              <span class="glyphicon glyphicon-arrow-right"></span>
            </button></div>
        </div>
      </div>
      <div class="row">
        <label class="col-xs-3 control-label">幅</label>
        <div class="col-xs-9" style="padding:0">
          <div class="col-xs-6">
            <button class="btn btn-default fill" type="button"
                    id="sub-narrow" value="1">▶狭く◀</button>
          </div>
          <div class="col-xs-6">
            <button class="btn btn-default fill" type="button"
                    id="sub-wide" value="1">◀広く▶</button>
          </div>
        </div>
      </div>
    </div>
  </div>
</div>
<?php
viewer();
?>
<div class="alert alert-warning">
融着機のローラーが画面下端にわずかに見える程度に、カメラ位置を調節してください。
</div>
<?php
bodyFoot();
