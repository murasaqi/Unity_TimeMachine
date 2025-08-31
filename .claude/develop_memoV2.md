# TimeMachineLTCBridgeの初期セットアップの方法変更
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