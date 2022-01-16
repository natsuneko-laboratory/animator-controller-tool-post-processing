# AnimatorControllerToolPostProcessing

Unity における以下の挙動を書き換えます。

* Animator Window を開いた状態で、新しく追加したレイヤーに対してデフォルト値として Weight = 1.0f を設定 (通常の挙動は Weight = 0.0f)
* Animator Window を開いた状態で、現在の StateMachine に対して追加された State の Write Defaults の値を False として設定 (通常の挙動は Write Defaults = True)

なお、このエディター拡張はスクリプトが読み込まれている (つまりはプロジェクト内に存在する) 限り常時有効となります。

## License

License Zero Parity 7.0.0 (see LICENSE-PARITY file) and MIT (contributions, see LICENSE-MIT file) with exception License Zero Patron 1.0.0 (see LICENSE-PATRON file)
