# LTC Decoderとの連携

## 開発目的
- Unity TimeMachineとPackageとして追加されているLTC Decoderと連携する役割をもつコンポーネントを開発したい
  - TimeMachineOSCReceiverが参考例
- LTCによるタイムラインの操作と、TimeMachineによるTimelineの操作どちらも受け付けるいいとこ取りができるComponentを開発したい
- LTC DecoderでTCを受信してるときはLTCDecoderとLTC Timeline SyncコンポーネントによるTimeline操作を優先する
  - TimeMachineの機能をMuteし、OSCから命令を無視する
- LTC DecoderがLTCを受信できてないときは、TimeMachine本来の機能とOSCによる制御を優先する
- Version Definesで専用のDefineを定義し、疎結合な連携にする
- OSCのプラグインが参考事例

## 開発されるコンポーネント
- TImeMachineLTCReceiver
  - Defineでの依存関係
    - LTC Decoder
    - uOSC
    - ExtOSC

## どのように連携するか
- TImeMachineLTCReceiver
  - LTCの受信に応じて、TimeMachineとOSCプラグインを操作する
- LTC DecoderがLTCを受信した時
  - TimeMachineのMute
  - TimeMachineのOSCプラグインが連携されていたら、Ignore OSC CommandsをTrueにする
- LTC DecoderがLTC受信がStopしたとき
  - TimeMachineをUnMute
  - OSCのIgnoreをFalseに
  - TimeMachineのUpdateClipsFinishStateByCurrentTimeを発火
- 各連携イベントはプログラム側でセットアップがすんでいるが、各イベントを実行できるかどうかはInspector側でToggleで設定することもできる



## TimeMachineLTCBridgeの初期セットアップの方法変更
TimeMachine LTC BridgeにAuto Setup Buttonを追加して、LTC Decoder、LTC Timeline Syncの設定を一括でできるようにしたいです。

- TimeMachineLTCBridgeをAdd
- Addされた直後はSetupがされていないので、Auto SetupのButtonが表示されている
- Auto Setup Buttonを押すと、参照関係の解決をおこなう
- TimeMachineControllerの取得
  - 同じGameObjectに存在していればGetしてSet
  - なければシーンを探して見つかればSet
- OSC Receiverの取得
  - 同じGameObjectに存在していればGetしてSet
  - 同じGameObjectになければ、シーンを探して見つかればSet
- LTC Timeline SyncとLTC Decoderの取得
  - LTC DecoderがScene内を探して見つかればSet
    - 見つからなかれば新規でGameObjectを作成し、LTC DecoderをAdd Componentする
  - LTC Timeline Syncをシーン内に探しても見つかればSet
    - 見つからなかれば同じGameObjectにLTC Timeline SyncをAddし、LTC Timeline Syncの参照にLTC DecoderをSet
    - PlayableDirectorはTimeMachineControllerがコントロールしているPlayableDirectorをSet