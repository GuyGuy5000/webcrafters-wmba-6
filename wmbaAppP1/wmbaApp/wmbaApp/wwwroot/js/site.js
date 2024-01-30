// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
// Done By: Emmanuel James
// Last Modified: 2024/01/23
// Write your JavaScript code.
var player = new Vimeo.Player(document.getElementById('videoFrame'));

player.setLoop(true);
player.setVolume(0);
player.play();