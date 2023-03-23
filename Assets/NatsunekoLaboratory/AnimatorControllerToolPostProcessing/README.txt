# AnimatorControllerToolPostProcessing

Unity における以下の挙動を書き換えます。

* Animator Window を開いた状態で、新しく追加したレイヤーに対してデフォルト値として Weight = 1.0f を設定 (通常の挙動は Weight = 0.0f)
* Animator Window を開いた状態で、現在の StateMachine に対して追加された State の Write Defaults の値を False として設定 (通常の挙動は Write Defaults = True)

なお、このエディター拡張はスクリプトが読み込まれている (つまりはプロジェクト内に存在する) 限り常時有効となります。

## 使い方

いれるだけで OK です。
もし機能を無効にしたい場合は、メニューバーの `NatsunekoLaboratory` → `Behaviours` の各項目から切り替えられます。


## 既知のバグ

名前長すぎるのでだれか良い名前考えてほしいです


## サポート

Discord までお越しください。

