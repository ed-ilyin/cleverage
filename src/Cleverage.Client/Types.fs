module Cleverage.Client.Types

type Символ = string
type Роль = Спаситель | Масон | Рептилоид | Демон
type Имя = string

type Действие =
    | ГоворитЧтоПолучилКлючНе of Роль * Символ
    | ГолосуетПротив of Символ
    | День of int
    | Зевнул
    | Ночь of int
    | ПовторноеГолосование
    | Подозревает of Символ
    | Подозрительно of string
    | ПолучилКлючНе of Роль * Символ
    | ПопалВРабствоКиНе of Роль list
    | ПорабощёнКак of Роль
    | ПорабощёнНе of Роль
    | ПризналсяЧто of Роль
    | Оказался of Роль
    | ОказалсяНе of Роль
    | ПризналсяЧтоНе of Роль
    | Пробасил
    | Пророчит of спаситель: Символ * рептиль1: Символ * рептиль2: Символ * демон: Символ
    | СказалПравду
    | Соврал
    | Топит of Символ
    | Щадит of Символ
    | ИзНегоВыбилиНе of Роль
    | СтавитНа of Символ
