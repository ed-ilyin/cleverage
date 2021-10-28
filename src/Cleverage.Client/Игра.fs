module Cleverage.Client.Игра
open Types

let игра = "DeepSkyBlue Status"
let я = "👨🏿‍🦰"

let игроки = [
    "😈", "Зомби"
    "👨‍👦‍👦", "Буратино"
    "♟️", "Жмых"
    "👨🏿‍🦰", "Карабас"
    "🦉", "Заморыш"
    "🍑", "Забава"
    "🕊", "Купидон"
    "👣", "Маркиза"
    "🤬", "Пупок"
    "😎", "Шайтан"
]

let события = [
    "", День
    "😈", ГолосуетПротив "🕊"
    "🤬", ГолосуетПротив "🕊"
    "🕊", ГолосуетПротив "🍑"
    "👨‍👦‍👦", ГолосуетПротив "👣"
    "👨🏿‍🦰", ГолосуетПротив "😈"
    "👣", ГолосуетПротив "♟️"
    "🦉", ГолосуетПротив "👨‍👦‍👦"
    "♟️", ГолосуетПротив "🤬"
    "😎", ГолосуетПротив "👣"
    "🍑", ГолосуетПротив "😈"
    "👣", ПорабощёнНе Демон
    "", Ночь
    "🤬", ПорабощёнНе Демон
    "👨‍👦‍👦", ПорабощёнНе Демон
    "👨‍👦‍👦", ПорабощёнНе Рептилоид
    "", День
    "🕊", ГолосуетПротив "♟️"
    "♟️", ГолосуетПротив "🕊"
    "😈", ГолосуетПротив "🕊"
    "😎", ГолосуетПротив "🕊"
    "🍑", ГолосуетПротив "👨🏿‍🦰"
    "🦉", ГолосуетПротив "😈"
    "👨🏿‍🦰", ГолосуетПротив "😈"
    "", ПовторноеГолосование
    "😈", ПорабощёнНе Рептилоид
    "", Ночь
    "🍑", ПорабощёнНе Демон
    "🍑", ПорабощёнНе Рептилоид
    "", День
    "😎", ПорабощёнНе Демон
    "", Ночь
    "♟️", ПорабощёнНе Демон
]
