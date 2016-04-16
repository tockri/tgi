(function($) {
    /**
     * Countdown Layer
     * @param {object} opt
     */
    function CountdownLayer(opt) {
        this.options = opt;
        this.init();
    }
    CountdownLayer.prototype = {
        membrane: null,
        base: null,
        message: null,
        counter: null,
        options: null,
        timer: 0,
        /**
         * コンストラクタ
         */
        init: function() {
            this.initMembrane();
            this.initBase();
            this.initMessage();
            this.initCounter();
            this.initButton();
            this.adjustBase();
        },
        /**
         * 半透明の膜
         */
        initMembrane: function() {
            var membrane = $('<div>'
            + '<div class="cd-base">'
            + '<div class="cd-message"></div>'
            + '<div class="cd-counter"></div>'
            + '<button type="button" class="cd-cancel-button">キャンセル</button>'
            + '</div>'
            + '</div>');
            if (this.options.hideCancel) {
                membrane.find('.cd-cancel-button').hide();
            }
            membrane.css({
                zIndex: 999,
                position: 'fixed',
                verticalAlign: 'middle',
                textAlign: 'center',
                top: 0,
                left: 0,
                right: 0,
                bottom: 0,
                background: 'rgba(0,0,0,0.8)',
                opacity: 0
            });
            $('body').append(membrane);
            this.membrane = membrane;
        },
        /**
         * コンテンツの箱
         */
        initBase: function() {
            var base = this.membrane.find('.cd-base');
            base.css({
                position: 'absolute',
                top: 0,
                left: 0
            });
            this.base = base;
        },
        /**
         * メッセージ
         */
        initMessage: function() {
            var message = this.base.find('.cd-message');
            message.css({
                fontSize: 24,
                color: 'white',
                paddingLeft: 10,
                paddingRight: 10
            });
            message.text(this.options.message);
            this.message = message;
        },
        /**
         * 数字
         */
        initCounter: function() {
            var counter = this.base.find('.cd-counter');
            var w = $(window);
            var winWidth = w.width();
            var winHeight = w.height();
            counter.css({
                fontSize: Math.max(winWidth, winHeight) / 4,
                fontFamily: 'Impact',
                color: 'white',
                marginTop: -10,
                marginBottom: -10
            });
            counter.text(this.options.count);
            this.counter = counter;
        },
        /**
         * キャンセルボタン
         */
        initButton: function() {
            var btn = this.base.find('.cd-cancel-button');
            btn.css({
                fontSize: 24,
                padding: 12
            });
            var l = this;
            btn.click(function() {
                if (l.timer) {
                    clearTimeout(l.timer);
                }
                l.membrane.remove();
                l.options.canceled && l.options.canceled();
            });
            this.button = btn;
        },
        /**
         * 箱の位置を調節する
         */
        adjustBase: function() {
            var base = this.base;
            var membrane = this.membrane;
            setTimeout(function() {
                var bw = base.width();
                var bh = base.height();
                base.css({
                    marginLeft: -bw / 2,
                    marginTop: -bh / 2,
                    top: '50%',
                    left: '50%'
                });
                membrane.css('opacity', 1);
            }, 50);
        }
    };
    
    $.countdown = function(opt) {
        opt = $.extend({
            count: 10,
            message: '',
            completed: null,
            canceled: null,
            dismissOnComplete: true,
            hideCancel: false
        }, opt);
        var cdl = new CountdownLayer(opt);
        var cnum = opt.count + 1;
        function cdInner() {
            cnum--;
            cdl.counter.text(cnum);
            if (cnum === 0) {
                cdl.button.hide();
                opt.completed && opt.completed();
                if (opt.dismissOnComplete) {
                    cdl.membrane.remove();
                }
            } else {
                cdl.timer = setTimeout(cdInner, 1000);
            }
        }
        cdInner();
    };    
})(jQuery);

