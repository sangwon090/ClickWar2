using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClickWar2.Game
{
    public static class GameValues
    {
        /// <summary>
        /// 영토의 힘을 증가시키는 명령의 최소 시간차(ms)
        /// </summary>
        public static int MinAddPowerDelay
        { get; } = 33;

        /// <summary>
        /// 영토를 공격하는 명령의 최소 시간차(ms)
        /// </summary>
        public static int MinAttackDelay
        { get; } = 1000;

        /// <summary>
        /// 시야 거리
        /// </summary>
        public static int VisionLength
        { get; } = 4;

        /// <summary>
        /// 힘을 전달하기 위해 필요한 최소 힘
        /// </summary>
        public static int MinPowerToSend
        { get; } = 2;

        /// <summary>
        /// 이벤트가 발생하기위한 최소 힘 크기
        /// </summary>
        public static int MinPowerToEvent
        { get; } = 1000;

        /// <summary>
        /// 큰 이벤트가 발생하기위한 최소 힘 크기
        /// </summary>
        public static int MinHugePowerToEvent
        { get; } = 10000;

        /// <summary>
        /// 푯말의 최대 허용길이
        /// </summary>
        public static int MaxSignLength
        { get; } = 32;

        /// <summary>
        /// 공장이 가동하기까지 걸리는 시간(ms)
        /// </summary>
        public static int MinRunFactoryDelay
        { get; } = 5000;

        /// <summary>
        /// 공장에서 한번에 변환되는 양
        /// </summary>
        public static int FactoryConversionAmount
        { get; } = 5;

        /// <summary>
        /// 자원 1당 변환되는 힘
        /// </summary>
        public static int ResourcePowerRate
        { get; } = 1;

        /// <summary>
        /// 회사를 등록하는데 필요한 자원량
        /// </summary>
        public static int CompanyRegistrationFee
        { get; } = 100;

        /// <summary>
        /// 회사 이름의 최대 허용 길이
        /// </summary>
        public static int MaxCompanyNameLength
        { get; } = 32;

        /// <summary>
        /// 회사를 건설하는데 필요한 자원량
        /// </summary>
        public static int CompanyBuildFee
        { get; } = 200;

        /// <summary>
        /// 회사 건물 하나당 가질 수 있는 기술 개수
        /// </summary>
        public static int CompanyTechSizePerSite
        { get; } = 1;

        /// <summary>
        /// 회사 건물 하나당 창고 크기
        /// </summary>
        public static int CompanyProductSizePerSite
        { get; } = 10;

        /// <summary>
        /// 프로그램 한줄당 개발 비용
        /// </summary>
        public static int DevFeePerProgramLine
        { get; } = 10;

        /// <summary>
        /// 프로그램 한줄당 생산 비용
        /// </summary>
        public static int ProduceFeePerProgramLine
        { get; } = 8;

        /// <summary>
        /// 칩이 한번 작동하기까지 걸리는 시간(ms)
        /// </summary>
        public static int MinRunChipDelay
        { get; } = 200;
    }
}
