var gulp = require("gulp");
var sass = require("gulp-sass");
var autoprefixer = require("gulp-autoprefixer");
var uglify = require("gulp-uglify");
var browser = require("browser-sync");
var plumber = require('gulp-plumber');
var webpack = require('webpack-stream');
var shell = require('gulp-shell');
var wait = require('gulp-wait');
var sourcemaps = require('gulp-sourcemaps');

// 
// webpack設定
var webpackConfig = {
    entry: {
        index: './js/index.js',
        config: './js/config.js',
        detail: './js/detail.js'
    },
    output: {
        filename: '[name].js'
    },
    resolve: {
        extensions: ['', '.js']
    }
};
// HTTPサーバ 3000ポートで待ち受ける
// クライアントのみ開発用
gulp.task("localserver", function() {
    browser({
       server: {baseDir: './www/'} // クライアント側の開発だけするときはこちら
    });
});
// アプリ側開発用
gulp.task("server", function() {
    browser({
        proxy: 'localhost'
    });
});

// Sassのコンパイル
gulp.task("sass", function() {
    gulp.src('./sass/**/*.scss')
        .pipe(plumber())
//        .pipe(sourcemaps.init())
        .pipe(sass({
            outputStyle: 'compressed'
        }))
        .pipe(autoprefixer())
//        .pipe(sourcemaps.write('./maps'))
        .pipe(gulp.dest('./www/css'))
        .pipe(browser.reload({stream: true}));
});

// Javascriptのビルド
gulp.task("js", function() {
    gulp.src('./js/**/*.js')
        .pipe(plumber())
        .pipe(webpack(webpackConfig))
        .pipe(uglify(webpackConfig.entry, {
            outSourceMap: false
        }))
        .pipe(gulp.dest('./www/js'))
        .pipe(browser.reload({stream: true}));
});

// bowerでインストールしたjQuery, bootstrapをコピーする
gulp.task('copy', function() {
    // CSS
    gulp.src(['./bower_components/**/dist/**/*.css',
            './bower_components/**/dist/**/*.css.map',
            './bower_components/**/dist/**/fonts/*'])
        .pipe(gulp.dest('./www/css'));
    // JavaScript
    gulp.src(['./bower_components/*/dist/**/*.js',
            './bower_components/*/dist/**/*.min.map'])
        .pipe(gulp.dest('./www/js'));
});


// PHPの変更によりHTMLを出力
gulp.task("php", function() {
    gulp.src('./php/*.php')
    .pipe(plumber())
        .pipe(shell([
            'php php/<%=fname(file)%>.php > www/<%=fname(file)%>.html'
        ], {
            templateData: {
                // 拡張子を除いたファイル名だけを返す
                fname: function(file) {
                    var m = file.path.match(/([^\/\\\.\s]+)(\.\w+)?$/);
                    return m[1];
                }
            }
        }))
        .pipe(wait(1000))
        .pipe(browser.reload({stream:true}));
});


// ファイルの変更監視
gulp.task('dev', ['copy', 'js', 'sass', 'php', 'server'], function() {
    gulp.watch(["./js/**/*.js"], ["js"]);
    gulp.watch(["./sass/**/*.scss"], ["sass"]);
    gulp.watch(["./php/**/*.php"], ['php']);
});

// localserverを使ったCSSなどの開発
gulp.task('local', ['copy', 'js', 'sass', 'php', 'localserver'], function() {
    gulp.watch(["./js/**/*.js"], ["js"]);
    gulp.watch(["./sass/**/*.scss"], ["sass"]);
    gulp.watch(["./php/**/*.php"], ['php']);
});

gulp.task('default', function() {
    console.log('dev or local');
});