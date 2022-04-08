import ForkTsCheckerWebpackPlugin from 'fork-ts-checker-webpack-plugin';
import path from 'path';
const CopyWebpackPlugin = require('copy-webpack-plugin');
import HtmlWebpackPlugin from 'html-webpack-plugin';
import MiniCssWebpackPlugin from 'mini-css-extract-plugin';

type Environment = {
    analyze: boolean;
};

type WebpackOptions = {
    mode: 'development' | 'production',
    watch: boolean
};

const statSettings = {
    builtAt: true,
    colors: true,
    errors: true,
    errorDetails: true,
    hash: true,
    timings: true,
    warnings: true,
    assets: false,
    cachedAssets: false,
    cached: false,
    children: false,
    chunks: false,
    entrypoints: false,
    modules: false,
    version: false,
};

const tsCheckerSelector = (options: WebpackOptions) => options.watch || options.mode === 'development' ?
    new ForkTsCheckerWebpackPlugin() :
    new ForkTsCheckerWebpackPlugin({ tslint: true });

const copyPlugin = new CopyWebpackPlugin ([
    {
        from: path.join(__dirname, 'node_modules', 'react', 'umd', 'react.production.min.js'),
        to: path.join(__dirname, 'bundles', 'lib', 'react.min.js')
    },
    {
        from: path.join(__dirname, 'node_modules', 'react-dom', 'umd', 'react-dom.production.min.js'),
        to: path.join(__dirname, 'bundles', 'lib', 'react-dom.min.js')
    }, {
        from: path.join(__dirname, 'node_modules', 'react', 'umd', 'react.development.js'),
        to: path.join(__dirname, 'bundles', 'lib', 'react.js')
    },
    {
        from: path.join(__dirname, 'node_modules', 'react-dom', 'umd', 'react-dom.development.js'),
        to: path.join(__dirname, 'bundles', 'lib', 'react-dom.js')
    },
    {
        from: path.join(__dirname, 'node_modules', 'react-bootstrap', 'dist', 'react-bootstrap.min.js'),
        to: path.join(__dirname, 'bundles', 'lib', 'react-bootstrap.min.js')
    },
]);

const clientOutput = {
    path: path.join(__dirname, '../NasladdinPlace.UI/wwwroot/js/CatalogPage'),
    filename: 'bundle.js',
    publicPath: '/',
};

const moduleSettings = {
    rules: [
        {
            test: /\.tsx?$/,
            loader: 'babel-loader',
            exclude: /node_modules/,
        },
        {
            test: /\.(css)$/,
            use: [
                MiniCssWebpackPlugin.loader,
                'css-loader',
            ],
        },
        {
            test: /\.(woff|woff2|svg|eot|ttf|png|jpg|gif)$/,
            use: [{
                loader: 'url-loader',
                options: {
                    limit: 10000
                }
            }]
        },
    ]
};

const resolve = { extensions: ['.ts', '.tsx', '.js', '.json'] };

const clientConfigFunc = (_: Environment | undefined, options: WebpackOptions) => {


    const htmlWebpack = options.mode === 'development'
        ? new HtmlWebpackPlugin({
            template: "./index.html"
        })
        : [];


    const plugins = [
        tsCheckerSelector(options), 
        copyPlugin, 
        new MiniCssWebpackPlugin({ filename: 'bundle.css' }),
    ].concat(htmlWebpack as any);

    const webpackConfig = {
        devServer: {
            headers: { 'Access-Control-Allow-Origin': '*' },
            https: false,
            disableHostCheck: true,
            historyApiFallback: true,
        },
        name: 'client',
        target: 'web',
        context: __dirname,
        stats: statSettings,
        entry: { client: ['./src/client.tsx'] },
        output: clientOutput,
        devtool: options.mode === 'development' ? 'inline-source-map' : undefined,
        module: moduleSettings,
        resolve: resolve,
        mode: options.mode,
        externals: options.mode === 'development' ? {} : {
            'react': 'React',
            'react-dom': 'ReactDOM',
        },
        plugins: plugins,
    };

    return webpackConfig;
};

module.exports = [clientConfigFunc];