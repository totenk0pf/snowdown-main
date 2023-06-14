namespace States.Zombie {
    public class ZombieDeadState : EnemyState {
        private bool _done;
        
        public override EnemyState RunCurrentState() {
            if (_done) return this;
            Agent.ResetPath();
            Anim.SetLayerWeight(1, 0f);
            Anim.SetTrigger("OnDeath");
            Anim.SetBool("Walking", false);
            Anim.SetBool("Attacking", false);
            Anim.SetBool("Dead", true);
            _done = true;
            return this;
        }
    }
}