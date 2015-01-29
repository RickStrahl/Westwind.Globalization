/// <binding AfterBuild='scripts' />
var gulp = require('gulp');

var merge = require('merge-stream');
var gulpIgnore = require('gulp-ignore');
//var del = require('del');

var concat = require('gulp-concat');
var uglify = require('gulp-uglify');
var sourcemaps = require('gulp-sourcemaps');
var minifyCSS = require('gulp-minify-css');

gulp.task('default', ['scripts'], function() {

});

gulp.task('scripts', function() {
    //del(['build']);

    var compressconcatjs = gulp
        .src(['wwwroot/app/js/app.js', 'wwwroot/app/js/**/*.js'])
        .pipe(sourcemaps.init())
        .pipe(concat('app.min.js'))
        .pipe(uglify())
        .pipe(sourcemaps.write("."))
        .pipe(gulp.dest('wwwroot/js'));
    
    var angularcompressconcatjs = gulp
        .src(['wwwroot/scripts/angular/angular.min.js',
        'wwwroot/scripts/angular/angular-route.min.js',
        'wwwroot/scripts/angular/angular-animate.min.js',
        'wwwroot/scripts/angular/angular-sanitize.min.js',
        'wwwroot/scripts/angular/angular-touch.min.js'
    ])
    .pipe(sourcemaps.init())
    .pipe(concat('angular-concat.min.js'))
    .pipe(sourcemaps.write("."))
    .pipe(gulp.dest('wwwroot/js'));

    var scriptcompressconcatjs = gulp
        .src(['wwwroot/scripts/lodash.min.js',
        'wwwroot/scripts/fastclick.min.js',
        'wwwroot/scripts/ww.angular.js',
        'wwwroot/scripts/ww.jquery.min.js',
        'wwwroot/scripts/bootstrap-typeahead.min.js'
        ])
    .pipe(sourcemaps.init())
    .pipe(concat('libraries.min.js'))
    .pipe(sourcemaps.write("."))
    .pipe(gulp.dest('wwwroot/js'));


    var concatjs = gulp
        .src(['wwwroot/app/js/app.js','wwwroot/app/js/**/*.js'])
        .pipe(concat('app.js'))
        .pipe(gulp.dest('wwwroot/js'));
   
    return merge(concatjs,compressconcatjs,angularcompressconcatjs, scriptcompressconcatjs);
});

gulp.task('css', function() {
    var concatcss = gulp.src(['css/bootstrap.css', 'css/bootstrap-theme.css', 'css/musicstore.css'])
       .pipe(gulpIgnore.exclude('**/*.min.*'))
       .pipe(concat('app.css'))
       .pipe(gulp.dest('build'));

    var concatCompressCss = gulp.src(['css/bootstrap.css', 'css/bootstrap-theme.css', 'font-awesome.css', 'css/musicstore.css'])
        .pipe(gulpIgnore.exclude('**/*.min.*'))
        .pipe(concat('app.min.css'))
        .pipe(minifyCSS({ keepBreaks: false }))
        .pipe(gulp.dest('build'));

    return merge( concatcss, concatCompressCss);
});
gulp.task('watch', function () {
    gulp.watch('wwwroot/app/js/**/*.js', ['scripts']);
    gulp.watch('wwwroot/css/*.css', ['css']);
});


