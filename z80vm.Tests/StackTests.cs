﻿using Xunit;

namespace z80vm.Tests
{
    public class StackTests
    {
        [Fact]
        public void OnANewMachineTheSPRegisterShouldContainTheHighestPossibleMemoryAddress()
        {
            var machine = new Machine();
            Assert.Equal(Memory.HIGHEST_ADDRESS, machine.Registers.Read(Reg16.SP));
        }

        [Fact]
        public void PushingAValueOnToTheStackShouldDecreaseTheSPRegisterByTwo()
        {
            var machine = new Machine();
            var currentValueOfSP = machine.Registers.Read(Reg16.SP);

            machine.PUSH(Reg16.AF);

            Assert.Equal(currentValueOfSP - 2, machine.Registers.Read(Reg16.SP));
        }

        [Theory]
        [InlineData(Reg16.BC)]
        public void PushingAValueOnToTheStackCopiesTheValueOfTheRegisterIntoTheMemoryLocationPointedToBySP(Reg16 register)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            var machine = new Machine();

            machine.Registers.Set(register, 0xABCD);
            machine.PUSH(register);
            var currentValueOfSP = machine.Registers.Read(Reg16.SP);

            Assert.Equal(0xAB, machine.Memory.Read((ushort)(currentValueOfSP + 1)));
            Assert.Equal(0xCD, machine.Memory.Read(currentValueOfSP));
        }

        [Fact]
        public void PoppingAValueFromToTheStackShouldIncreaseTheSPRegisterByTwo()
        {
            var machine = new Machine();
            machine.PUSH(Reg16.AF);

            var currentValueOfSP = machine.Registers.Read(Reg16.SP);
            machine.POP(Reg16.AF);

            Assert.Equal(currentValueOfSP + 2, machine.Registers.Read(Reg16.SP));
        }

        [Theory]
        [InlineData(Reg16.AF)]
        [InlineData(Reg16.BC)]
        [InlineData(Reg16.DE)]
        [InlineData(Reg16.HL)]
        [InlineData(Reg16.IX)]
        [InlineData(Reg16.IY)]
        public void PoppingAValueFromTheStackCopiesTheMemoryLocationPointedToBySPToTheRegister(Reg16 register)
        {
            //The Z80 is little endian,  so the lowest byte is stored in the lowest address
            var machine = new Machine();
            machine.Registers.Set(Reg16.SP, 0x1000);
            machine.Memory.Set(0x1000 + 1, 0xAA);  //High order byte
            machine.Memory.Set(0x1000, 0xBB);  //Low order byte

            machine.POP(register);

            Assert.Equal(0xAABB, machine.Registers.Read(register));
        }

        [Theory]
        [InlineData(Reg16.AF)]
        [InlineData(Reg16.BC)]
        [InlineData(Reg16.DE)]
        [InlineData(Reg16.HL)]
        [InlineData(Reg16.IX)]
        [InlineData(Reg16.IY)]
        public void ShouldBeAbleToPOPaPUSHedValue(Reg16 register)
        {
            var machine = new Machine();
            machine.Registers.Set(register, 0xAAAA);
            machine.PUSH(register);
            machine.Registers.Set(register, 0xBBBB);
            machine.POP(register);

            Assert.Equal(0xAAAA, machine.Registers.Read(register));
        }
    }
}
