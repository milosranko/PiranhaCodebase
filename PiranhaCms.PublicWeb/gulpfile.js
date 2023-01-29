var gulp = require('gulp'),
    cleanCSS = require('gulp-clean-css'),
    rename = require('gulp-rename'),
    concat = require('gulp-concat'),
    minify = require('gulp-minify');

gulp.task('minify-css', () => {
    return gulp.src('wwwroot/assets/css/style.css')
        .pipe(cleanCSS())
        .pipe(rename({ suffix: ".min" }))
        .pipe(gulp.dest('wwwroot/assets/css'));
});

gulp.task('minify-js', () => {
    return gulp.src('wwwroot/assets/js/main.js')
        .pipe(minify({ ext: { min: '.min.js' } }))
        .pipe(gulp.dest('wwwroot/assets/js'));
});

gulp.task('bundle-css', () => {
    return gulp.src(['wwwroot/assets/css/bootstrap.min.css', 'wwwroot/lib/owlcarousel/assets/owl.carousel.min.css', 'wwwroot/assets/css/style.min.css'])
        .pipe(concat('styles.bundle.css'))
        .pipe(gulp.dest('wwwroot/assets/css'));
});

gulp.task('bundle-js', () => {
    return gulp.src(['wwwroot/lib/jquery/jquery-3.6.3.min.js', 'wwwroot/lib/jquery-validation/dist/jquery.validate.min.js', 'wwwroot/lib/jquery-validation-unobtrusive/jquery.validate.unobtrusive.min.js', 'wwwroot/lib/easing/easing.min.js', 'wwwroot/lib/waypoints/waypoints.min.js', 'wwwroot/lib/owlcarousel/owl.carousel.min.js', 'wwwroot/assets/js/main.min.js'])
        .pipe(concat('scripts.bundle.js'))
        .pipe(gulp.dest('wwwroot/assets/js'));
});
