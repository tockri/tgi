<?php
class SiteForm {
    const LABEL_COLS = 2;
    
    function label($label) {
        return '<label class="col-sm-' . self::LABEL_COLS . ' control-label">' . $label . '</label>';
    }
    
    /**
     * テキスト入力
     * @param string $name
     * @param string $label
     * @param bool $req
     * @param int $maxlength
     */
    function text($name, $label, $req = false, $half = false, $clearfix = false, $maxlength = 100) {
        $html = '<div class="siteform-group' . ($clearfix ? ' clearfix' : '') . '">';
        $html .= $this->label($label);
        $cols = $half ? 6 - self::LABEL_COLS  : 12 - self::LABEL_COLS;
        $html .= '<div class="col-sm-' . $cols . '">'
                . '<div class="input-group">'
                . '<input type="text" name="' . $name . '"'
                . ($req ? ' required' : '')
                . ' class="form-control" maxlength="' . $maxlength . '">'
                . '<span class="input-group-addon btn btn-default">'
                . '<span class="glyphicon glyphicon-saved" aria-hidden="true"></span>'
                . '</span>'
                . '</div>'
                . '</div>';
        $html .= '</div>';
        echo $html;
    }
    /**
     * 複数行テキスト入力
     * @param string $name
     * @param string $label
     * @param bool $req
     */
    function textarea($name, $label, $req = false, $half = false, $clearfix = false) {
        $html = '<div class="siteform-group' . ($clearfix ? ' clearfix' : '') . '">';
        $html .= $this->label($label);
        $cols = $half ? 6 - self::LABEL_COLS  : 12 - self::LABEL_COLS;
        $html .= '<div class="col-sm-' . $cols . '">'
                . '<div class="input-group">'
                . '<textarea name="' . $name . '" rows="3"'
                . ($req ? ' required' : '')
                . ' class="form-control custom-control"></textarea>'
                . '<span class="input-group-addon btn btn-default">'
                . '<span class="glyphicon glyphicon-saved" aria-hidden="true"></span>'
                . '</span>'
                . '</div>'
                . '</div>';
        $html .= '</div>';
        echo $html;
    }
    /**
     * ON / OFFのラジオボタン
     * @param string $name
     * @param string $label
     */
    function onoff($name, $label, $half = false, $clearfix = false) {
        $html = '<div class="siteform-group' . ($clearfix ? ' clearfix' : '') . '">';
        $html .= $this->label($label);
        $cols = $half ? 6 - self::LABEL_COLS  : 12 - self::LABEL_COLS;
        $html .= '<div class="col-sm-' . $cols . '">';
        foreach (array('1' => 'ON', '0' => 'OFF') as $v => $l) {
            $html .= '<label class="radio-inline">'
                    . '<input type="radio" name="' . $name . '"'
                    . ' value="' . $v . '" />'
                    . $l
                    . '</label>';
        }
        $html .= '</div>';
        $html .= '</div>';
        echo $html;
    }
    
    /**
     * selectのガワ
     * @param string $name
     * @param string $label
     * @param bool $req
     */
    function select($name, $label, $req = false, $half = false, $clearfix = false) {
        $html = '<div class="siteform-group' . ($clearfix ? ' clearfix' : '') . '">';
        $html .= $this->label($label);
        $cols = $half ? 6 - self::LABEL_COLS  : 12 - self::LABEL_COLS;
        $html .= '<div class="col-sm-' . $cols . '">';
        $html .= '<select class="form-control"'
                . ' id="select-' . $name . '"'
                . ($req ? ' required' : '')
                . ' name="' . $name . '"></select>';
        $html .= '</div>';
        $html .= '</div>';
        echo $html;
    }
    
}
$s = new SiteForm();
?>
<!-- サイト属性form -->
<form action="javascript:void(0)" id="form-siteform">
  <div class="form-horizontal">
    <input type="hidden" name="DirName" value="" />
    <?php
    $s->text('SiteName', 'サイト名', true);
    $s->onoff('CsvOutput', 'CSV出力', true);
    $s->onoff('ImageOutput', 'エラー画像出力', true);
    $s->onoff('AlermSound', 'アラーム音', true);
    $s->select('CofFile', 'cofファイル', true, true);
    $s->select('ThresholdSet', '閾値判定変数', true, true);
    $s->select('ColorMap', '色温度マップ', true, true);
    $s->text('SitePerson', '接合者名', false, true);
    $s->text('SheetType', 'シートタイプ', false, true);
    $s->text('FusionTemp', '接合温度', false, true);
    $s->text('FusionSpeed', '接合速度', false, true);
    $s->text('FusionPressure', '圧力', false, true, true);
    $s->textarea('Memo', 'メモ');
    ?>
  </div>
</form>