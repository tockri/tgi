<?php
require 'code/pagebase.php';
htmlHead('detail');
bodyHead('detail');
?>
<div id="site-select-pane" class="container">
  <div class="form-group">
    <label for="site-select" class="col-xs-12">サイトの選択</label>
    <table class="site-select-table">
        <tr>
            <td>
                <select id="site-select" class="form-control"></select>
            </td>
            <td>
              <span id="site-duplicate-button"
                    class="btn btn-info">
                新規サイト設定
              </span>
            </td>
        </tr>
    </table>
  </div>
</div>
<div id="detail-pane">
  <ul>
    <li id="tab-site">サイト属性</li>
    <li id="tab-thermo">カメラ温度</li>
  </ul>
  <div id="config-site" class="tab-pane container">
      <?php require 'code/siteform.php'; ?>
  </div>
  <div id="config-temp" class="tab-pane container">
    <?php require 'code/signaltemp.php'; ?>
  </div>
</div>
<?php
viewer();
?>
<div class="restart-pane">
  <button type="button" class="btn btn-warning btn-lg" id="restart-button">
      <span class="glyphicon glyphicon-refresh"></span>
      再起動
  </button>
  <div class="message">カメラに異常が発生した場合など、アプリケーションを再起動します。</div>
</div>
<div class="shutdown-pane">
  <button type="button" class="btn btn-danger btn-lg"
          id="shutdown-button">
    <span class="glyphicon glyphicon-off"></span>
    シャットダウン
  </button>
  <div class="message">注意！融着機側のPCをシャットダウンします。</div>
</div>
<?php
bodyFoot();
