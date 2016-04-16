TGI2

遮水シート熱画像検査アプリケーション

このプロジェクトについて

[TGI2アプリケーション]部と、[HTMLクライアント]部からなっています。

[TGI2アプリケーション]は.Net Framework3.5に依存し、AIR 32カメラSDKのdllの制限により、
32bit環境でしか実行できません。また、本番の環境はWindows XPです。
そのため、Windows XP 32bitのVMを起動して、その中で開発することになります。


[HTMLクライアント]は、HTMLとCSSとJavaScriptのフロントエンドです。
TGI2アプリケーションが起動すると80番ポートでHTTPを待ち受け、8002番ポートでWebSocketを
待ち受けます。

PHPなどのようにサーバーサイドで動的にHTMLを生成することはありません。
すべての動的な挙動はクライアント側JavaScript、サーバー側JSON API＋WebSocketによって実現します。




