module Cleverage.Client.Игра
open Types

let игра = "DeepSkyBlue Status"
let я = "👨🏿‍🦰"

let игроки = [
    "👣", "Маркиза"
    "👨🏿‍🦰", "Карабас"
    "😎", "Шайтан"
    "🤖", "Бендер"
    "♟", "Жмых"
    "👨‍👦‍👦", "Буратино"
    "🦍", "Примат"
    "😈", "Штопор"
    "К", "Купидон"
    "🍑", "Забава"
]

let события = [
    "👣", Подозрительно "будет слушать и делать выводы"
    "♟", Подозрительно "грыз семки"
    "🤖", Подозрительно "передразнил Маркизу"
    "🍑", Подозрительно "не хочет порабощать"
    "👨‍👦‍👦", Подозрительно "на карантине"
    "😈", Подозрительно "забыл русский"
    "К", Подозрительно "перепутал имя"
    "", День
    "😈", ГолосуетПротив "👨🏿‍🦰"
    "♟", ГолосуетПротив "👨🏿‍🦰"
    "👨‍👦‍👦", ГолосуетПротив "🤖"
    "😎", ГолосуетПротив "🤖"
    "👨🏿‍🦰", ГолосуетПротив "👣"
    "🦍", ГолосуетПротив "К"
    "🤖", ГолосуетПротив "♟"
    "👣", ГолосуетПротив "👨🏿‍🦰"
    "🍑", ГолосуетПротив "🦍"
    "К", ГолосуетПротив "👨🏿‍🦰"
    "", ПовторноеГолосование
    "👨‍👦‍👦", Щадит "👨🏿‍🦰"
    "🍑", Щадит "👨🏿‍🦰"
    "🤖", ГолосуетПротив "👨🏿‍🦰"
    "🦍", Щадит "👨🏿‍🦰"
    "К", ГолосуетПротив "👨🏿‍🦰"
    "😎", Щадит "👨🏿‍🦰"
    "👣", Щадит "👨🏿‍🦰"
    "😈", ГолосуетПротив "👨🏿‍🦰"
    "", Ночь
    "🍑", ПорабощёнНе Рептилоид
    "🍑", ПорабощёнНе Демон
    "", День
    "🦍", ГолосуетПротив "👨‍👦‍👦"
    "К", ГолосуетПротив "😈"
    "👨🏿‍🦰", ГолосуетПротив "😈"
    "🤖", ГолосуетПротив "😎"
    "👣", ГолосуетПротив "К"
    "👨‍👦‍👦", ГолосуетПротив "🦍"
    "😈", ГолосуетПротив "♟"
    "♟", ГолосуетПротив "🦍"
    "😎", ГолосуетПротив "🤖"
    "🦍", ПорабощёнНе Демон
    "", Ночь
    "👨‍👦‍👦", ПорабощёнНе Демон
    "👨‍👦‍👦", ПорабощёнНе Рептилоид
    "🦍", ПорабощёнНе Спаситель
    "", День
]
