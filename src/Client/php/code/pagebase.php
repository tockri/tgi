<?php
/*
 * ページレイアウトの基本部分
 */
ini_set('display_errors', 'On');

/**
 * URLとタイトルの関連付け
 * @return type
 */
function nameLabels() {
    return array(
        'index' => '記録',
        'config' => '監視枠',
        'detail' => '詳細設定',
    );
}

/**
 * <html><head>〜</head>
 * @param type $name
 */
function htmlHead($name) {
    $menues = nameLabels();
    echo <<<END
<!DOCTYPE HTML>
<html lang="ja-JP">
  <head>
    <meta charset="UTF-8">
<!--
   ！注意！ このHTMLはPHPを利用して生成されています。
   HTMLファイルを直接編集しないでください。
-->
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width,initial-scale=1.0,maximum-scale=1.0,user-scalable=0">
    <link rel="stylesheet" href="css/bootstrap/dist/css/bootstrap.min.css" />
    <link rel="stylesheet" href="css/common.css" />
    <link rel="stylesheet" href="css/{$name}.css" />
    <script type="text/javascript" src="js/jquery/dist/jquery.min.js"></script>
    <script type="text/javascript" src="js/bootstrap/dist/js/bootstrap.min.js"></script>
    <script type="text/javascript" src="js/{$name}.js"></script>
    <title>TGI2:{$menues[$name]}</title>
  </head>
END;
}
/**
 * <body>〜ヘッダメニュー
 */
function bodyHead($name) {
    $menues = nameLabels();
    echo <<<END
<body>
    <div class="heading">
      <table class="heading">
        <tr>
END;
    foreach ($menues as $k => $label) {
        echo '<td class="col-xs-4">';
        if ($k === $name) {
            echo '<div class="fill active btn btn-default">' . $label . '</div>';
        } else {
            echo '<a href="' . $k . '.html" class="fill btn btn-default">' . $label . '</a>';
        }
        echo'</td>';
    }
    echo <<<END
        </tr>
      </table>
    </div>
    <div class="membrane"></div>
    <div id="message-pane"></div>
END;
}

function viewer() {
    echo <<<END
      <div id="image-pane">
        <div class="viewer">
          <img src="/img/viewer-noimage.gif" alt="" width="320" height="240" id="thermo-image" />
          <div id="watchbox-main">
            <div id="watchbox1-1" class="watchbox main"></div>
            <div id="watchbox1-2" class="watchbox main"></div>
          </div>
          <div id="watchbox-sub">
            <div id="watchbox2" class="watchbox sub"></div>
          </div>
        </div>
        <div class="colormap">
          <img src="/img/colormap-noimage.gif" alt="" width="320" height="40" id="colormap-image" />
        </div>
      </div>
END;
}


function bodyFoot() {
    echo <<<END
  </body>
</html>
END;
}
